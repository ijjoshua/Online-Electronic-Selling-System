using System.Security.Cryptography;
using MySqlConnector;
using OnlineElectronicSellingSystem.Data;
using OnlineElectronicSellingSystem.Models;

namespace OnlineElectronicSellingSystem.Repositories;

public sealed class OessRepository
{
    private readonly Database database;
    public OessRepository(Database database) => this.database = database;

    public User? Authenticate(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Enter an email address.");
        if (string.IsNullOrEmpty(password)) throw new ArgumentException("Enter a password.");
        using var connection = database.OpenConnection();
        using var command = new MySqlCommand("SELECT id,full_name,email,password_hash,role,status FROM users WHERE email=@email", connection);
        command.Parameters.AddWithValue("@email", email.Trim().ToLowerInvariant());
        using var reader = command.ExecuteReader();
        if (!reader.Read()) throw new InvalidOperationException($"No account was found for '{email.Trim()}'.");
        var status = reader.GetString("status");
        if (!status.Equals("Active", StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException($"This account is not active. Current status: {status}.");
        if (!VerifyPassword(password, reader.GetString("password_hash"))) throw new InvalidOperationException("The password is incorrect.");
        return new User { Id = reader.GetInt32("id"), FullName = reader.GetString("full_name"), Email = reader.GetString("email"), Role = Enum.Parse<UserRole>(reader.GetString("role"), true) };
    }

    public void RegisterCustomer(string fullName, string email, string password)
    {
        if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) || password.Length < 8)
            throw new ArgumentException("Name and email are required, and the password must contain at least 8 characters.");

        using var connection = database.OpenConnection();
        using var command = new MySqlCommand("INSERT INTO users(full_name,email,password_hash,role) VALUES(@name,@email,@password,'Customer')", connection);
        command.Parameters.AddWithValue("@name", fullName.Trim());
        command.Parameters.AddWithValue("@email", email.Trim().ToLowerInvariant());
        command.Parameters.AddWithValue("@password", PasswordHasher.Hash(password));
        try { command.ExecuteNonQuery(); }
        catch (MySqlException ex) when (ex.Number == 1062) { throw new InvalidOperationException("An account with this email already exists.", ex); }
    }

    public IReadOnlyList<Product> GetProducts(string? search = null)
    {
        using var connection = database.OpenConnection();
        using var command = new MySqlCommand(@"SELECT id,sku,name,brand_id,category_id,category,description,warranty_months,status,price,stock_quantity,reorder_level,is_featured,is_new_arrival,is_active
FROM products WHERE is_active=1 AND (@search='' OR name LIKE @like OR sku LIKE @like OR category LIKE @like) ORDER BY name", connection);
        var value = search?.Trim() ?? "";
        command.Parameters.AddWithValue("@search", value);
        command.Parameters.AddWithValue("@like", $"%{value}%");
        using var reader = command.ExecuteReader();
        var result = new List<Product>();
        while (reader.Read()) result.Add(ReadProduct(reader));
        return result;
    }

    public void SaveProduct(Product product)
    {
        using var connection = database.OpenConnection();
        using var command = new MySqlCommand(@"INSERT INTO products(sku,name,brand_id,category_id,category,description,warranty_months,status,price,stock_quantity,reorder_level,is_featured,is_new_arrival,is_active)
VALUES(@sku,@name,@brand,@categoryId,@category,@description,@warranty,@status,@price,@stock,@reorder,@featured,@newArrival,@active)
ON DUPLICATE KEY UPDATE name=@name,brand_id=@brand,category_id=@categoryId,category=@category,description=@description,warranty_months=@warranty,status=@status,price=@price,stock_quantity=@stock,reorder_level=@reorder,is_featured=@featured,is_new_arrival=@newArrival,is_active=@active", connection);
        command.Parameters.AddWithValue("@sku", product.Sku); command.Parameters.AddWithValue("@name", product.Name);
        command.Parameters.AddWithValue("@brand", (object?)product.BrandId ?? DBNull.Value); command.Parameters.AddWithValue("@categoryId", (object?)product.CategoryId ?? DBNull.Value);
        command.Parameters.AddWithValue("@category", product.Category); command.Parameters.AddWithValue("@price", product.Price);
        command.Parameters.AddWithValue("@description", (object?)product.Description ?? DBNull.Value); command.Parameters.AddWithValue("@warranty", product.WarrantyMonths);
        command.Parameters.AddWithValue("@status", product.Status); command.Parameters.AddWithValue("@stock", product.StockQuantity); command.Parameters.AddWithValue("@reorder", product.ReorderLevel);
        command.Parameters.AddWithValue("@featured", product.IsFeatured); command.Parameters.AddWithValue("@newArrival", product.IsNewArrival); command.Parameters.AddWithValue("@active", product.IsActive);
        command.ExecuteNonQuery();
    }

    public void AddToCart(int userId, int productId, int quantity)
    {
        using var connection = database.OpenConnection();
        using var transaction = connection.BeginTransaction();
        using var stock = new MySqlCommand("SELECT stock_quantity FROM products WHERE id=@id AND is_active=1 FOR UPDATE", connection, transaction);
        stock.Parameters.AddWithValue("@id", productId);
        var available = Convert.ToInt32(stock.ExecuteScalar() ?? 0);
        if (quantity < 1 || quantity > available) throw new InvalidOperationException($"Only {available} unit(s) are available.");
        using var command = new MySqlCommand(@"INSERT INTO cart_items(user_id,product_id,quantity) VALUES(@user,@product,@quantity)
ON DUPLICATE KEY UPDATE quantity=quantity+@quantity", connection, transaction);
        command.Parameters.AddWithValue("@user", userId); command.Parameters.AddWithValue("@product", productId); command.Parameters.AddWithValue("@quantity", quantity);
        command.ExecuteNonQuery(); transaction.Commit();
    }

    public IReadOnlyList<CartItem> GetCart(int userId)
    {
        using var connection = database.OpenConnection();
        using var command = new MySqlCommand(@"SELECT p.id,p.sku,p.name,p.price,c.quantity FROM cart_items c JOIN products p ON p.id=c.product_id WHERE c.user_id=@user ORDER BY p.name", connection);
        command.Parameters.AddWithValue("@user", userId); using var reader = command.ExecuteReader();
        var result = new List<CartItem>();
        while (reader.Read()) result.Add(new CartItem { ProductId=reader.GetInt32("id"), Sku=reader.GetString("sku"), ProductName=reader.GetString("name"), UnitPrice=reader.GetDecimal("price"), Quantity=reader.GetInt32("quantity") });
        return result;
    }

    public void RemoveFromCart(int userId, int productId)
    {
        using var connection = database.OpenConnection(); using var command = new MySqlCommand("DELETE FROM cart_items WHERE user_id=@user AND product_id=@product", connection);
        command.Parameters.AddWithValue("@user", userId); command.Parameters.AddWithValue("@product", productId); command.ExecuteNonQuery();
    }

    public string Checkout(int userId, string paymentMethod)
    {
        using var connection = database.OpenConnection(); using var transaction = connection.BeginTransaction();
        try
        {
            var items = new List<(int ProductId, int Quantity, decimal Price)>();
            using (var command = new MySqlCommand("SELECT c.product_id,c.quantity,p.price,p.stock_quantity FROM cart_items c JOIN products p ON p.id=c.product_id WHERE c.user_id=@user FOR UPDATE", connection, transaction))
            { command.Parameters.AddWithValue("@user", userId); using var reader = command.ExecuteReader(); while (reader.Read()) { var quantity=reader.GetInt32("quantity"); var stock=reader.GetInt32("stock_quantity"); if (quantity>stock) throw new InvalidOperationException($"Insufficient stock for product {reader.GetInt32("product_id")}"); items.Add((reader.GetInt32("product_id"),quantity,reader.GetDecimal("price"))); } }
            if (items.Count == 0) throw new InvalidOperationException("The cart is empty.");
            var total = items.Sum(x => x.Price * x.Quantity); var orderNumber = $"OESS-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
            using var order = new MySqlCommand(@"INSERT INTO orders(order_number,user_id,total,status,payment_method,payment_status) VALUES(@number,@user,@total,'Confirmed',@method,IF(@method='Cash on Delivery','Pending','Paid')); SELECT LAST_INSERT_ID();", connection, transaction);
            order.Parameters.AddWithValue("@number", orderNumber); order.Parameters.AddWithValue("@user", userId); order.Parameters.AddWithValue("@total", total); order.Parameters.AddWithValue("@method", paymentMethod);
            var orderId = Convert.ToInt32(order.ExecuteScalar());
            foreach (var item in items)
            {
                using var line = new MySqlCommand("INSERT INTO order_items(order_id,product_id,quantity,unit_price) VALUES(@order,@product,@quantity,@price); UPDATE products SET stock_quantity=stock_quantity-@quantity WHERE id=@product;", connection, transaction);
                line.Parameters.AddWithValue("@order", orderId); line.Parameters.AddWithValue("@product", item.ProductId); line.Parameters.AddWithValue("@quantity", item.Quantity); line.Parameters.AddWithValue("@price", item.Price); line.ExecuteNonQuery();
            }
            using var clear = new MySqlCommand("DELETE FROM cart_items WHERE user_id=@user", connection, transaction); clear.Parameters.AddWithValue("@user", userId); clear.ExecuteNonQuery(); transaction.Commit(); return orderNumber;
        }
        catch { transaction.Rollback(); throw; }
    }

    public IReadOnlyList<OrderSummary> GetOrders(int? userId = null)
    {
        using var connection = database.OpenConnection(); using var command = new MySqlCommand(@"SELECT o.id,o.order_number,u.full_name,o.total,o.status,o.order_date FROM orders o JOIN users u ON u.id=o.user_id WHERE (@user=0 OR o.user_id=@user) ORDER BY o.order_date DESC", connection);
        command.Parameters.AddWithValue("@user", userId ?? 0); using var reader=command.ExecuteReader(); var result=new List<OrderSummary>();
        while(reader.Read()) result.Add(new OrderSummary { Id=reader.GetInt32("id"), OrderNumber=reader.GetString("order_number"), CustomerName=reader.GetString("full_name"), Total=reader.GetDecimal("total"), Status=reader.GetString("status"), OrderDate=reader.GetDateTime("order_date") }); return result;
    }

    public void UpdateOrderStatus(int orderId, string status)
    {
        using var connection = database.OpenConnection();
        using var command = new MySqlCommand("UPDATE orders SET status=@status WHERE id=@id", connection);
        command.Parameters.AddWithValue("@status", status); command.Parameters.AddWithValue("@id", orderId); command.ExecuteNonQuery();
    }

    public void UpdateCartQuantity(int userId, int productId, int quantity)
    {
        if (quantity < 1) throw new ArgumentException("Quantity must be at least one.");
        using var connection = database.OpenConnection();
        using var command = new MySqlCommand(@"UPDATE cart_items ci JOIN products p ON p.id=ci.product_id
SET ci.quantity=@quantity,ci.unit_price=p.price
WHERE ci.user_id=@user AND ci.product_id=@product AND p.is_active=1 AND @quantity<=p.stock_quantity", connection);
        command.Parameters.AddWithValue("@quantity", quantity); command.Parameters.AddWithValue("@user", userId); command.Parameters.AddWithValue("@product", productId);
        if (command.ExecuteNonQuery() == 0) throw new InvalidOperationException("The requested quantity is not available.");
    }

    public ReportSummary GetReport()
    {
        using var connection=database.OpenConnection(); using var command=new MySqlCommand(@"SELECT
(SELECT COUNT(*) FROM products WHERE is_active=1) total_products,
(SELECT COUNT(*) FROM users WHERE role='Customer' AND status='Active') total_customers,
(SELECT COUNT(*) FROM orders) total_orders,
(SELECT COALESCE(SUM(total),0) FROM orders WHERE payment_status='Paid') total_sales,
(SELECT COUNT(*) FROM suppliers WHERE is_active=1) total_suppliers,
(SELECT COUNT(*) FROM support_tickets WHERE status IN ('Open','InProgress')) pending_support_tickets,
(SELECT COUNT(*) FROM orders WHERE order_date >= CURDATE()) orders_today,
(SELECT COALESCE(SUM(total),0) FROM orders WHERE order_date >= CURDATE() AND payment_status='Paid') revenue_today,
(SELECT COUNT(*) FROM orders WHERE status IN ('Pending','Confirmed','Processing')) pending_orders,
(SELECT COUNT(*) FROM products WHERE stock_quantity <= reorder_level AND is_active=1) low_stock,
(SELECT COUNT(*) FROM users WHERE role='Customer' AND status='Active') customers",connection); using var reader=command.ExecuteReader(); reader.Read();
        return new ReportSummary { TotalProducts=reader.GetInt32("total_products"), TotalCustomers=reader.GetInt32("total_customers"), TotalOrders=reader.GetInt32("total_orders"), TotalSales=reader.GetDecimal("total_sales"), TotalSuppliers=reader.GetInt32("total_suppliers"), PendingSupportTickets=reader.GetInt32("pending_support_tickets"), OrdersToday=reader.GetInt32("orders_today"), RevenueToday=reader.GetDecimal("revenue_today"), PendingOrders=reader.GetInt32("pending_orders"), LowStockProducts=reader.GetInt32("low_stock"), Customers=reader.GetInt32("customers") };
    }

    private static Product ReadProduct(MySqlDataReader reader) => new() { Id=reader.GetInt32("id"), Sku=reader.GetString("sku"), Name=reader.GetString("name"), BrandId=reader.IsDBNull(reader.GetOrdinal("brand_id")) ? null : reader.GetInt32("brand_id"), CategoryId=reader.IsDBNull(reader.GetOrdinal("category_id")) ? null : reader.GetInt32("category_id"), Category=reader.GetString("category"), Description=reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString("description"), WarrantyMonths=reader.GetInt32("warranty_months"), Status=reader.GetString("status"), Price=reader.GetDecimal("price"), StockQuantity=reader.GetInt32("stock_quantity"), ReorderLevel=reader.GetInt32("reorder_level"), IsFeatured=reader.GetBoolean("is_featured"), IsNewArrival=reader.GetBoolean("is_new_arrival"), IsActive=reader.GetBoolean("is_active") };
    private static bool VerifyPassword(string password, string stored)
    {
        if (!stored.StartsWith("pbkdf2:", StringComparison.Ordinal)) return stored == password;
        var parts=stored.Split(':'); var salt=Convert.FromBase64String(parts[1]); var expected=Convert.FromBase64String(parts[2]); var actual=Rfc2898DeriveBytes.Pbkdf2(password,salt,120000,HashAlgorithmName.SHA256,expected.Length); return CryptographicOperations.FixedTimeEquals(actual,expected);
    }
}
