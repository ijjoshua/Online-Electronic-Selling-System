using MySqlConnector;
using OnlineElectronicSellingSystem.Data;
using OnlineElectronicSellingSystem.Models;

namespace OnlineElectronicSellingSystem.Repositories;

public sealed class InventoryRepository
{
    private readonly Database database;
    public InventoryRepository(Database database) => this.database = database;

    public IReadOnlyList<InventoryRecord> GetInventory(string? search)
    {
        using var connection = database.OpenConnection();
        using var command = new MySqlCommand(@"SELECT id,sku,name,stock_quantity,reorder_level FROM products
WHERE is_active=1 AND (@search='' OR sku LIKE @like OR name LIKE @like) ORDER BY name", connection);
        var value = search?.Trim() ?? "";
        command.Parameters.AddWithValue("@search", value); command.Parameters.AddWithValue("@like", $"%{value}%");
        using var reader = command.ExecuteReader();
        var result = new List<InventoryRecord>();
        while (reader.Read()) result.Add(new InventoryRecord { ProductId = reader.GetInt32("id"), Sku = reader.GetString("sku"), ProductName = reader.GetString("name"), StockQuantity = reader.GetInt32("stock_quantity"), ReorderLevel = reader.GetInt32("reorder_level") });
        return result;
    }

    public IReadOnlyList<StockMovementRecord> GetMovements(int? productId = null)
    {
        using var connection = database.OpenConnection();
        using var command = new MySqlCommand(@"SELECT m.id,m.product_id,p.name,m.movement_type,m.quantity,m.reason,m.reference_type,m.reference_id,m.created_at
FROM stock_movements m JOIN products p ON p.id=m.product_id
WHERE (@productId=0 OR m.product_id=@productId) ORDER BY m.created_at DESC", connection);
        command.Parameters.AddWithValue("@productId", productId ?? 0);
        using var reader = command.ExecuteReader();
        var result = new List<StockMovementRecord>();
        while (reader.Read()) result.Add(new StockMovementRecord
        {
            Id = reader.GetInt32("id"), ProductId = reader.GetInt32("product_id"), ProductName = reader.GetString("name"),
            MovementType = reader.GetString("movement_type"), Quantity = reader.GetInt32("quantity"), Reason = reader.GetString("reason"),
            ReferenceType = reader.IsDBNull(reader.GetOrdinal("reference_type")) ? null : reader.GetString("reference_type"),
            ReferenceId = reader.IsDBNull(reader.GetOrdinal("reference_id")) ? null : reader.GetInt32("reference_id"), CreatedAt = reader.GetDateTime("created_at")
        });
        return result;
    }

    public void AdjustStock(int productId, int delta, string movementType, string reason, int userId)
    {
        if (delta == 0) throw new ArgumentException("The stock adjustment cannot be zero.");
        using var connection = database.OpenConnection();
        using var transaction = connection.BeginTransaction();
        try
        {
            using var stock = new MySqlCommand("SELECT stock_quantity FROM products WHERE id=@id AND is_active=1 FOR UPDATE", connection, transaction);
            stock.Parameters.AddWithValue("@id", productId);
            var current = Convert.ToInt32(stock.ExecuteScalar() ?? throw new InvalidOperationException("The product was not found."));
            if (current + delta < 0) throw new InvalidOperationException("Stock cannot become negative.");
            using var update = new MySqlCommand("UPDATE products SET stock_quantity=stock_quantity+@delta WHERE id=@id", connection, transaction);
            update.Parameters.AddWithValue("@delta", delta); update.Parameters.AddWithValue("@id", productId); update.ExecuteNonQuery();
            using var movement = new MySqlCommand(@"INSERT INTO stock_movements(product_id,user_id,movement_type,quantity,reason)
VALUES(@product,@user,@type,@quantity,@reason)", connection, transaction);
            movement.Parameters.AddWithValue("@product", productId); movement.Parameters.AddWithValue("@user", userId); movement.Parameters.AddWithValue("@type", movementType);
            movement.Parameters.AddWithValue("@quantity", delta); movement.Parameters.AddWithValue("@reason", reason.Trim()); movement.ExecuteNonQuery();
            transaction.Commit();
        }
        catch { transaction.Rollback(); throw; }
    }
}
