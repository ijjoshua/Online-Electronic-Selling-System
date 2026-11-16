namespace OnlineElectronicSellingSystem.Models;

public enum UserRole { Customer, Admin, Seller, SalesOfficer, SupportAgent, DeliveryAgent }
public sealed class User
{
    public int Id { get; init; }
    public string FullName { get; init; } = "";
    public string Email { get; init; } = "";
    public UserRole Role { get; init; }
}
