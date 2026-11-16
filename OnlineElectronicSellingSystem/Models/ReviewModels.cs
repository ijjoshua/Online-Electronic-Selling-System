namespace OnlineElectronicSellingSystem.Models;

public sealed class ReviewRecord
{
    public int Id { get; init; }
    public string ProductName { get; init; } = "";
    public string CustomerName { get; init; } = "";
    public int Rating { get; init; }
    public string? Comment { get; init; }
    public string Status { get; init; } = "";
    public bool VerifiedBuyer { get; init; }
    public DateTime CreatedAt { get; init; }
}

public sealed class ProductQuestionRecord
{
    public int Id { get; init; }
    public string ProductName { get; init; } = "";
    public string CustomerName { get; init; } = "";
    public string Question { get; init; } = "";
    public string? Answer { get; init; }
    public string Status { get; init; } = "";
    public DateTime CreatedAt { get; init; }
}
