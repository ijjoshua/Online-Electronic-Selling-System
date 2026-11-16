namespace OnlineElectronicSellingSystem.Models;

public sealed class UserRecord
{
    public int Id { get; init; }
    public string FullName { get; init; } = "";
    public string Email { get; init; } = "";
    public string? Phone { get; init; }
    public string Role { get; init; } = "Customer";
    public string Status { get; init; } = "Active";
    public DateTime CreatedAt { get; init; }
}
