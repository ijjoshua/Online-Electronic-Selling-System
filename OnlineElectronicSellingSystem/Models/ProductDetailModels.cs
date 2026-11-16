namespace OnlineElectronicSellingSystem.Models;

public sealed class ProductVariantRecord { public int Id { get; init; } public int ProductId { get; init; } public string ProductName { get; init; } = ""; public string Sku { get; init; } = ""; public string? Attributes { get; init; } public decimal Price { get; init; } public int StockQuantity { get; init; } public int ReorderLevel { get; init; } public bool IsActive { get; init; } }
public sealed class ProductSpecificationRecord { public int Id { get; init; } public int ProductId { get; init; } public string ProductName { get; init; } = ""; public string SpecificationName { get; init; } = ""; public string SpecificationValue { get; init; } = ""; }
public sealed class ProductImageRecord { public int Id { get; init; } public int ProductId { get; init; } public string ProductName { get; init; } = ""; public string ImageUrl { get; init; } = ""; public bool IsMain { get; init; } public int DisplayOrder { get; init; } }
