namespace OnlineElectronicSellingSystem.Models;

public sealed class SupplierManagementRecord
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public string? ContactName { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? Address { get; init; }
    public bool IsActive { get; init; }
}

public sealed class PurchaseOrderRecord
{
    public int Id { get; init; }
    public int SupplierId { get; init; }
    public string SupplierName { get; init; } = "";
    public string PurchaseOrderNumber { get; init; } = "";
    public string Status { get; init; } = "";
    public DateTime? OrderedAt { get; init; }
    public DateTime? ReceivedAt { get; init; }
    public string? Notes { get; init; }
}
