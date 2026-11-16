using OnlineElectronicSellingSystem.Models;
using OnlineElectronicSellingSystem.Repositories;

namespace OnlineElectronicSellingSystem.Controllers;

public sealed class SupplierController
{
    private readonly SupplierRepository repository;
    public SupplierController(SupplierRepository repository)=>this.repository=repository;
    private static void Staff(User u){if(u.Role is not(UserRole.Admin or UserRole.Seller))throw new UnauthorizedAccessException("Supplier access is restricted to administrators and sellers.");}
    public IReadOnlyList<SupplierManagementRecord> Suppliers(User u){Staff(u);return repository.Suppliers();}
    public void Save(User u,SupplierManagementRecord s){Staff(u);if(string.IsNullOrWhiteSpace(s.Name))throw new ArgumentException("Supplier name is required.");repository.SaveSupplier(s);}
    public IReadOnlyList<PurchaseOrderRecord> PurchaseOrders(User u){Staff(u);return repository.PurchaseOrders();}
    public void UpdatePurchaseOrderStatus(User u,int id,string status){Staff(u);repository.UpdatePurchaseOrderStatus(id,status);}
    public void CreatePurchaseOrder(User u,int supplierId,string number,int productId,int quantity,decimal unitCost,string? notes){Staff(u);if(string.IsNullOrWhiteSpace(number))throw new ArgumentException("Purchase-order number is required.");repository.CreatePurchaseOrder(supplierId,number,productId,quantity,unitCost,notes);}
}
