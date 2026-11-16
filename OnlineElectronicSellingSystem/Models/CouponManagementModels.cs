namespace OnlineElectronicSellingSystem.Models;

public sealed class CouponManagementRecord
{
    public int Id { get; init; }
    public string Code { get; init; } = "";
    public string DiscountType { get; init; } = "";
    public decimal Value { get; init; }
    public decimal MinimumOrderAmount { get; init; }
    public int? UsageLimit { get; init; }
    public int UsageCount { get; init; }
    public DateTime ValidFrom { get; init; }
    public DateTime ValidUntil { get; init; }
    public bool IsActive { get; init; }
}
