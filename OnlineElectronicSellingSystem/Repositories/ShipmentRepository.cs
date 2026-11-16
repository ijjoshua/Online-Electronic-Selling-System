using MySqlConnector;
using OnlineElectronicSellingSystem.Data;
using OnlineElectronicSellingSystem.Models;

namespace OnlineElectronicSellingSystem.Repositories;

public sealed class ShipmentRepository
{
    private readonly Database database;
    public ShipmentRepository(Database database) => this.database = database;

    public IReadOnlyList<ShipmentRecord> GetShipments()
    {
        using var connection = database.OpenConnection();
        using var command = new MySqlCommand(@"SELECT s.id,s.order_id,o.order_number,s.courier,s.delivery_agent_id,u.full_name AS delivery_agent_name,
s.tracking_number,s.delivery_method,s.status,s.estimated_delivery,s.delivered_at,s.delivery_note
FROM shipments s JOIN orders o ON o.id=s.order_id LEFT JOIN users u ON u.id=s.delivery_agent_id ORDER BY s.id DESC", connection);
        using var reader = command.ExecuteReader(); var result = new List<ShipmentRecord>();
        while (reader.Read()) result.Add(new ShipmentRecord { Id=reader.GetInt32("id"), OrderId=reader.GetInt32("order_id"), OrderNumber=reader.GetString("order_number"), Courier=reader.IsDBNull(reader.GetOrdinal("courier"))?null:reader.GetString("courier"), DeliveryAgentId=reader.IsDBNull(reader.GetOrdinal("delivery_agent_id"))?null:reader.GetInt32("delivery_agent_id"), DeliveryAgentName=reader.IsDBNull(reader.GetOrdinal("delivery_agent_name"))?null:reader.GetString("delivery_agent_name"), TrackingNumber=reader.IsDBNull(reader.GetOrdinal("tracking_number"))?null:reader.GetString("tracking_number"), DeliveryMethod=reader.GetString("delivery_method"), Status=reader.GetString("status"), EstimatedDelivery=reader.IsDBNull(reader.GetOrdinal("estimated_delivery"))?null:reader.GetDateTime("estimated_delivery"), DeliveredAt=reader.IsDBNull(reader.GetOrdinal("delivered_at"))?null:reader.GetDateTime("delivered_at"), DeliveryNote=reader.IsDBNull(reader.GetOrdinal("delivery_note"))?null:reader.GetString("delivery_note") });
        return result;
    }

    public void UpdateStatus(int id, string status, string? trackingNumber, string? note)
    {
        using var connection = database.OpenConnection();
        using var command = new MySqlCommand("UPDATE shipments SET status=@status,tracking_number=@tracking,delivery_note=@note,delivered_at=IF(@status='Delivered',COALESCE(delivered_at,CURRENT_TIMESTAMP),NULL) WHERE id=@id", connection);
        command.Parameters.AddWithValue("@status", status); command.Parameters.AddWithValue("@tracking", (object?)trackingNumber ?? DBNull.Value); command.Parameters.AddWithValue("@note", (object?)note ?? DBNull.Value); command.Parameters.AddWithValue("@id", id); command.ExecuteNonQuery();
    }
}
