using MySqlConnector;
using OnlineElectronicSellingSystem.Data;
using OnlineElectronicSellingSystem.Models;

namespace OnlineElectronicSellingSystem.Repositories;

public sealed class WishlistRepository
{
    private readonly Database database;
    public WishlistRepository(Database database) => this.database = database;

    public IReadOnlyList<WishlistItemRecord> GetItems(int userId)
    {
        using var connection = database.OpenConnection();
        using var command = new MySqlCommand(@"SELECT p.id,p.sku,p.name,p.category,p.price,p.stock_quantity,wi.added_at
FROM wishlist_items wi JOIN wishlists w ON w.id=wi.wishlist_id JOIN products p ON p.id=wi.product_id
WHERE w.user_id=@user ORDER BY wi.added_at DESC", connection);
        command.Parameters.AddWithValue("@user", userId);
        using var reader = command.ExecuteReader();
        var result = new List<WishlistItemRecord>();
        while (reader.Read()) result.Add(new WishlistItemRecord { ProductId = reader.GetInt32("id"), Sku = reader.GetString("sku"), ProductName = reader.GetString("name"), Category = reader.GetString("category"), Price = reader.GetDecimal("price"), StockQuantity = reader.GetInt32("stock_quantity"), AddedAt = reader.GetDateTime("added_at") });
        return result;
    }

    public void Add(int userId, int productId)
    {
        using var connection = database.OpenConnection();
        using var command = new MySqlCommand(@"INSERT INTO wishlists(user_id) VALUES(@user)
ON DUPLICATE KEY UPDATE id=LAST_INSERT_ID(id);
INSERT IGNORE INTO wishlist_items(wishlist_id,product_id) VALUES(LAST_INSERT_ID(),@product)", connection);
        command.Parameters.AddWithValue("@user", userId); command.Parameters.AddWithValue("@product", productId); command.ExecuteNonQuery();
    }

    public void Remove(int userId, int productId)
    {
        using var connection = database.OpenConnection();
        using var command = new MySqlCommand("DELETE wi FROM wishlist_items wi JOIN wishlists w ON w.id=wi.wishlist_id WHERE w.user_id=@user AND wi.product_id=@product", connection);
        command.Parameters.AddWithValue("@user", userId); command.Parameters.AddWithValue("@product", productId); command.ExecuteNonQuery();
    }
}
