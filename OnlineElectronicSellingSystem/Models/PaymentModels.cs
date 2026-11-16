namespace OnlineElectronicSellingSystem.Models;

public sealed class PaymentRecord
{
    public int Id { get; init; }
    public int OrderId { get; init; }
    public string OrderNumber { get; init; } = "";
    public string Method { get; init; } = "";
    public decimal Amount { get; init; }
    public string Status { get; init; } = "";
    public string? Reference { get; init; }
    public DateTime? PaidAt { get; init; }
    public DateTime CreatedAt { get; init; }
}

public sealed class InvoiceRecord
{
    public int Id { get; init; }
    public int OrderId { get; init; }
    public string OrderNumber { get; init; } = "";
    public string InvoiceNumber { get; init; } = "";
    public string? PdfUrl { get; init; }
    public DateTime IssuedAt { get; init; }
}
