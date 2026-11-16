namespace OnlineElectronicSellingSystem.Models;

public sealed class StoreSettingRecord
{
    public string SettingKey { get; init; } = "";
    public string? SettingValue { get; init; }
    public int? UpdatedBy { get; init; }
    public DateTime UpdatedAt { get; init; }
}
