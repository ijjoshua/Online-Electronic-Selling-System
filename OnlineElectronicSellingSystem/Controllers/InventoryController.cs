using OnlineElectronicSellingSystem.Models;
using OnlineElectronicSellingSystem.Repositories;

namespace OnlineElectronicSellingSystem.Controllers;

public sealed class InventoryController
{
    private readonly InventoryRepository repository;
    public InventoryController(InventoryRepository repository) => this.repository = repository;

    private static void RequireStaff(User user)
    {
        if (user.Role is not (UserRole.Admin or UserRole.Seller)) throw new UnauthorizedAccessException("Only administrators and sellers can manage inventory.");
    }

    public IReadOnlyList<InventoryRecord> Inventory(User user, string? search) { RequireStaff(user); return repository.GetInventory(search); }
    public IReadOnlyList<StockMovementRecord> Movements(User user, int? productId = null) { RequireStaff(user); return repository.GetMovements(productId); }
    public void Adjust(User user, int productId, int delta, string type, string reason) { RequireStaff(user); repository.AdjustStock(productId, delta, type, reason, user.Id); }
}
