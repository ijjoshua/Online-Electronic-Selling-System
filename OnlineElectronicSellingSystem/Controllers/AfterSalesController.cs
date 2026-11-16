using OnlineElectronicSellingSystem.Models;
using OnlineElectronicSellingSystem.Repositories;

namespace OnlineElectronicSellingSystem.Controllers;

public sealed class AfterSalesController
{
    private readonly AfterSalesRepository repository;
    public AfterSalesController(AfterSalesRepository repository)=>this.repository=repository;
    private static void Staff(User u){if(u.Role is not(UserRole.Admin or UserRole.SupportAgent or UserRole.SalesOfficer))throw new UnauthorizedAccessException("After-sales access is restricted.");}
    public IReadOnlyList<ReturnRecord> Returns(User u){Staff(u);return repository.Returns();}
    public IReadOnlyList<WarrantyRecord> Warranties(User u){Staff(u);return repository.Warranties();}
    public IReadOnlyList<WarrantyClaimRecord> Claims(User u){Staff(u);return repository.Claims();}
    public void UpdateReturn(User u,int id,string status){Staff(u);repository.UpdateReturn(id,status);}
    public void UpdateWarranty(User u,int id,string status){Staff(u);repository.UpdateWarranty(id,status);}
    public void UpdateClaim(User u,int id,string status,string? resolution){Staff(u);repository.UpdateClaim(id,status,resolution);}
}
