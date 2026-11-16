using OnlineElectronicSellingSystem.Models;
using OnlineElectronicSellingSystem.Repositories;

namespace OnlineElectronicSellingSystem.Controllers;

public sealed class CatalogController
{
    private readonly CatalogRepository repository;
    public CatalogController(CatalogRepository repository) => this.repository = repository;

    private static void RequireStaff(User user)
    {
        if (user.Role is not (UserRole.Admin or UserRole.Seller)) throw new UnauthorizedAccessException("Only administrators and sellers can manage the catalogue.");
    }

    public IReadOnlyList<Brand> Brands(User user) { RequireStaff(user); return repository.GetBrands(); }
    public void SaveBrand(User user, Brand brand) { RequireStaff(user); repository.SaveBrand(brand); }
    public void SetBrandActive(User user, int id, bool active) { RequireStaff(user); repository.SetBrandActive(id, active); }
    public IReadOnlyList<Category> Categories(User user) { RequireStaff(user); return repository.GetCategories(); }
    public void SaveCategory(User user, Category category) { RequireStaff(user); repository.SaveCategory(category); }
    public void SetCategoryActive(User user, int id, bool active) { RequireStaff(user); repository.SetCategoryActive(id, active); }
}
