using OnlineElectronicSellingSystem.Models;
using OnlineElectronicSellingSystem.Repositories;

namespace OnlineElectronicSellingSystem.Controllers;

public sealed class ShipmentController
{
    private readonly ShipmentRepository repository;
    public ShipmentController(ShipmentRepository repository) => this.repository = repository;
    private static void RequireStaff(User user) { if (user.Role is not (UserRole.Admin or UserRole.DeliveryAgent or UserRole.SalesOfficer)) throw new UnauthorizedAccessException("Shipment access is restricted."); }
    public IReadOnlyList<ShipmentRecord> Shipments(User user) { RequireStaff(user); return repository.GetShipments(); }
    public void UpdateStatus(User user, int id, string status, string? tracking, string? note) { RequireStaff(user); repository.UpdateStatus(id,status,tracking,note); }
}
