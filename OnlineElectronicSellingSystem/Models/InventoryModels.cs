namespace OnlineElectronicSellingSystem.Models;

public sealed class InventoryRecord
{
    public int ProductId { get; init; }
    public string Sku { get; init; } = "";
    public string ProductName { get; init; } = "";
    public int StockQuantity { get; init; }
    public int ReorderLevel { get; init; }
    public string StockStatus => StockQuantity <= 0 ? "Out of stock" : StockQuantity <= ReorderLevel ? "Low stock" : "In stock";
}

public sealed class StockMovementRecord
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public string ProductName { get; init; } = "";
    public string MovementType { get; init; } = "";
    public int Quantity { get; init; }
    public string Reason { get; init; } = "";
    public string? ReferenceType { get; init; }
    public int? ReferenceId { get; init; }
    public DateTime CreatedAt { get; init; }
}
