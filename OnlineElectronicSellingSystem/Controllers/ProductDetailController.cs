using OnlineElectronicSellingSystem.Models;
using OnlineElectronicSellingSystem.Repositories;

namespace OnlineElectronicSellingSystem.Controllers;

public sealed class ProductDetailController
{
    private readonly ProductDetailRepository repository;
    public ProductDetailController(ProductDetailRepository repository)=>this.repository=repository;
    private static void Staff(User u){if(u.Role is not(UserRole.Admin or UserRole.Seller))throw new UnauthorizedAccessException("Product detail access is restricted.");}
    public IReadOnlyList<ProductVariantRecord> Variants(User u){Staff(u);return repository.Variants();}
    public IReadOnlyList<ProductSpecificationRecord> Specifications(User u){Staff(u);return repository.Specifications();}
    public IReadOnlyList<ProductImageRecord> Images(User u){Staff(u);return repository.Images();}
    public void AddSpecification(User u,int id,string name,string value){Staff(u);if(string.IsNullOrWhiteSpace(name)||string.IsNullOrWhiteSpace(value))throw new ArgumentException("Specification name and value are required.");repository.AddSpecification(id,name,value);}
    public void AddImage(User u,int id,string url,bool main,int order){Staff(u);if(string.IsNullOrWhiteSpace(url))throw new ArgumentException("Image URL is required.");repository.AddImage(id,url,main,order);}
}
