namespace OnlineElectronicSellingSystem.Models;

public sealed class AddressRecord
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public string UserName { get; init; } = "";
    public string Recipient { get; init; } = "";
    public string Street { get; init; } = "";
    public string City { get; init; } = "";
    public string? Region { get; init; }
    public string Country { get; init; } = "";
    public string? Phone { get; init; }
    public bool IsDefault { get; init; }
    public DateTime CreatedAt { get; init; }
}
