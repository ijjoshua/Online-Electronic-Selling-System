namespace OnlineElectronicSellingSystem.Models;

public sealed class SupportTicketRecord
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public string CustomerName { get; init; } = "";
    public string? AssignedAgent { get; init; }
    public string? OrderNumber { get; init; }
    public string Subject { get; init; } = "";
    public string Category { get; init; } = "";
    public string Status { get; init; } = "";
    public DateTime CreatedAt { get; init; }
}

public sealed class TicketMessageRecord
{
    public int Id { get; init; }
    public int TicketId { get; init; }
    public string UserName { get; init; } = "";
    public string Message { get; init; } = "";
    public DateTime CreatedAt { get; init; }
}

public sealed class NotificationRecord
{
    public int Id { get; init; }
    public int? UserId { get; init; }
    public string Channel { get; init; } = "";
    public string NotificationType { get; init; } = "";
    public string? Subject { get; init; }
    public string Message { get; init; } = "";
    public string DeliveryStatus { get; init; } = "";
    public DateTime CreatedAt { get; init; }
}
