namespace OnlineElectronicSellingSystem.Models;

public sealed class CustomerRecord
{
    public int Id { get; init; }
    public string FullName { get; init; } = "";
    public string Email { get; init; } = "";
    public string? Phone { get; init; }
    public string Status { get; init; } = "";
    public DateTime CreatedAt { get; init; }
}

public sealed class SupplierRecord
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public string? ContactName { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public bool IsActive { get; init; }
}

public sealed class CouponRecord
{
    public int Id { get; init; }
    public string Code { get; init; } = "";
    public string DiscountType { get; init; } = "";
    public decimal Value { get; init; }
    public decimal MinimumOrderAmount { get; init; }
    public DateTime ValidFrom { get; init; }
    public DateTime ValidUntil { get; init; }
    public bool IsActive { get; init; }
}

public sealed class AuditRecord
{
    public long Id { get; init; }
    public string? UserName { get; init; }
    public string Action { get; init; } = "";
    public string? EntityType { get; init; }
    public int? EntityId { get; init; }
    public DateTime CreatedAt { get; init; }
}
