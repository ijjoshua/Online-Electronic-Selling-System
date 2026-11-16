namespace OnlineElectronicSellingSystem.Models;

public sealed class PromotionRecord
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public string? Description { get; init; }
    public string DiscountType { get; init; } = "";
    public decimal DiscountValue { get; init; }
    public DateTime ValidFrom { get; init; }
    public DateTime ValidUntil { get; init; }
    public bool IsActive { get; init; }
}
