namespace OnlineElectronicSellingSystem.Models;

public sealed class SalesReportRecord { public string OrderNumber { get; init; } = ""; public string CustomerName { get; init; } = ""; public decimal Total { get; init; } public string PaymentStatus { get; init; } = ""; public DateTime OrderDate { get; init; } }
public sealed class PaymentReportRecord { public string OrderNumber { get; init; } = ""; public string Method { get; init; } = ""; public decimal Amount { get; init; } public string Status { get; init; } = ""; public DateTime CreatedAt { get; init; } }
public sealed class InventoryReportRecord { public string Sku { get; init; } = ""; public string ProductName { get; init; } = ""; public int StockQuantity { get; init; } public int ReorderLevel { get; init; } public string Status => StockQuantity <= 0 ? "Out of stock" : StockQuantity <= ReorderLevel ? "Low stock" : "In stock"; }
