namespace OnlineElectronicSellingSystem.Models;

public sealed class ShipmentRecord
{
    public int Id { get; init; }
    public int OrderId { get; init; }
    public string OrderNumber { get; init; } = "";
    public string? Courier { get; init; }
    public int? DeliveryAgentId { get; init; }
    public string? DeliveryAgentName { get; init; }
    public string? TrackingNumber { get; init; }
    public string DeliveryMethod { get; init; } = "Standard";
    public string Status { get; init; } = "Pending";
    public DateTime? EstimatedDelivery { get; init; }
    public DateTime? DeliveredAt { get; init; }
    public string? DeliveryNote { get; init; }
}
