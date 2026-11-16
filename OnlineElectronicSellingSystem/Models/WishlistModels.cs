namespace OnlineElectronicSellingSystem.Models;

public sealed class WishlistItemRecord
{
    public int ProductId { get; init; }
    public string Sku { get; init; } = "";
    public string ProductName { get; init; } = "";
    public string Category { get; init; } = "";
    public decimal Price { get; init; }
    public int StockQuantity { get; init; }
    public DateTime AddedAt { get; init; }
}
