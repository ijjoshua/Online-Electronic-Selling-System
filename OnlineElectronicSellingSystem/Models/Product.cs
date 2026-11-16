namespace OnlineElectronicSellingSystem.Models;

public sealed class Product
{
    public int Id { get; init; }
    public string Sku { get; init; } = "";
    public string Name { get; init; } = "";
    public string Category { get; init; } = "";
    public int? BrandId { get; init; }
    public int? CategoryId { get; init; }
    public string? Description { get; init; }
    public int WarrantyMonths { get; init; }
    public string Status { get; init; } = "Published";
    public decimal Price { get; init; }
    public int StockQuantity { get; init; }
    public int ReorderLevel { get; init; }
    public bool IsActive { get; init; }
    public bool IsFeatured { get; init; }
    public bool IsNewArrival { get; init; }
}

public sealed class CartItem
{
    public int ProductId { get; init; }
    public string Sku { get; init; } = "";
    public string ProductName { get; init; } = "";
    public decimal UnitPrice { get; init; }
    public int Quantity { get; set; }
    public decimal Total => UnitPrice * Quantity;
}

public sealed class OrderSummary
{
    public int Id { get; init; }
    public string OrderNumber { get; init; } = "";
    public string CustomerName { get; init; } = "";
    public decimal Total { get; init; }
    public string Status { get; init; } = "";
    public DateTime OrderDate { get; init; }
}
