namespace OnlineElectronicSellingSystem.Models;

public sealed class ReturnRecord
{
    public int Id { get; init; }
    public string? RmaNumber { get; init; }
    public string OrderNumber { get; init; } = "";
    public string ProductName { get; init; } = "";
    public string CustomerName { get; init; } = "";
    public string Reason { get; init; } = "";
    public string Status { get; init; } = "";
    public DateTime RequestedAt { get; init; }
}

public sealed class WarrantyRecord
{
    public int Id { get; init; }
    public string OrderNumber { get; init; } = "";
    public string ProductName { get; init; } = "";
    public string? SerialNumber { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public string Status { get; init; } = "";
}

public sealed class WarrantyClaimRecord
{
    public int Id { get; init; }
    public int WarrantyId { get; init; }
    public string CustomerName { get; init; } = "";
    public string ProductName { get; init; } = "";
    public string Description { get; init; } = "";
    public string Status { get; init; } = "";
    public DateTime CreatedAt { get; init; }
}
