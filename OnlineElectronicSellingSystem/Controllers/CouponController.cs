using OnlineElectronicSellingSystem.Models;
using OnlineElectronicSellingSystem.Repositories;

namespace OnlineElectronicSellingSystem.Controllers;

public sealed class CouponController
{
    private readonly CouponRepository repository;
    public CouponController(CouponRepository repository)=>this.repository=repository;
    private static void Admin(User u){if(u.Role!=UserRole.Admin)throw new UnauthorizedAccessException("Administrator access is required.");}
    public IReadOnlyList<CouponManagementRecord> All(User u){Admin(u);return repository.GetAll();}
    public void Save(User u,CouponManagementRecord c){Admin(u);if(string.IsNullOrWhiteSpace(c.Code)||c.ValidUntil<=c.ValidFrom)throw new ArgumentException("Coupon code and valid dates are required.");repository.Save(c);}
}
