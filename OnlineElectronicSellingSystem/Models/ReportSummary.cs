namespace OnlineElectronicSellingSystem.Models;

public sealed class ReportSummary
{
    public int TotalProducts { get; init; }
    public int TotalCustomers { get; init; }
    public int TotalOrders { get; init; }
    public decimal TotalSales { get; init; }
    public int TotalSuppliers { get; init; }
    public int PendingSupportTickets { get; init; }
    public int OrdersToday { get; init; }
    public decimal RevenueToday { get; init; }
    public int PendingOrders { get; init; }
    public int LowStockProducts { get; init; }
    public int Customers { get; init; }
}
