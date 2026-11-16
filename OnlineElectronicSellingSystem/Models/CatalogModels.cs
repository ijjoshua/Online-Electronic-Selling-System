namespace OnlineElectronicSellingSystem.Models;

public sealed class Brand
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public string? LogoUrl { get; init; }
    public string? Description { get; init; }
    public bool IsActive { get; init; }
}

public sealed class Category
{
    public int Id { get; init; }
    public int? ParentId { get; init; }
    public string Name { get; init; } = "";
    public string? Description { get; init; }
    public int DisplayOrder { get; init; }
    public bool IsActive { get; init; }
}
