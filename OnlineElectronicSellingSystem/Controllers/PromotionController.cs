using OnlineElectronicSellingSystem.Models;
using OnlineElectronicSellingSystem.Repositories;

namespace OnlineElectronicSellingSystem.Controllers;

public sealed class PromotionController
{
    private readonly PromotionRepository repository;
    public PromotionController(PromotionRepository repository)=>this.repository=repository;
    private static void Admin(User u){if(u.Role!=UserRole.Admin)throw new UnauthorizedAccessException("Administrator access is required.");}
    public IReadOnlyList<PromotionRecord> All(User u){Admin(u);return repository.GetAll();}
    public void Save(User u,PromotionRecord p){Admin(u);if(string.IsNullOrWhiteSpace(p.Name)||p.ValidUntil<=p.ValidFrom)throw new ArgumentException("Promotion name and valid dates are required.");repository.Save(p);}
}
