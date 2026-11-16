using OnlineElectronicSellingSystem.Models;
using OnlineElectronicSellingSystem.Repositories;

namespace OnlineElectronicSellingSystem.Controllers;

public sealed class AuthController
{
    private readonly OessRepository repository;
    public AuthController(OessRepository repository) => this.repository = repository;
    public User? Login(string email, string password) => repository.Authenticate(email, password);
    public void RegisterCustomer(string fullName, string email, string password) => repository.RegisterCustomer(fullName, email, password);
}

public sealed class ProductController
{
    private readonly OessRepository repository;
    public ProductController(OessRepository repository) => this.repository = repository;
    public IReadOnlyList<Product> Search(string? search) => repository.GetProducts(search);
    public void Save(Product product, User user)
    {
        if (user.Role is not (UserRole.Admin or UserRole.Seller)) throw new UnauthorizedAccessException("Only administrators and sellers can manage products.");
        repository.SaveProduct(product);
    }
}

public sealed class SalesController
{
    private readonly OessRepository repository;
    public SalesController(OessRepository repository) => this.repository = repository;
    public IReadOnlyList<CartItem> Cart(User user) => repository.GetCart(user.Id);
    public void AddToCart(User user, int productId, int quantity) => repository.AddToCart(user.Id, productId, quantity);
    public void RemoveFromCart(User user, int productId) => repository.RemoveFromCart(user.Id, productId);
    public void UpdateCartQuantity(User user, int productId, int quantity) => repository.UpdateCartQuantity(user.Id, productId, quantity);
    public string Checkout(User user, string paymentMethod) => repository.Checkout(user.Id, paymentMethod);
    public IReadOnlyList<OrderSummary> Orders(User user) => repository.GetOrders(user.Role == UserRole.Customer ? user.Id : null);
    public void UpdateOrderStatus(User user, int orderId, string status)
    {
        if (user.Role is not (UserRole.Admin or UserRole.SalesOfficer or UserRole.Seller)) throw new UnauthorizedAccessException("Order status access is restricted.");
        repository.UpdateOrderStatus(orderId, status);
    }
}

public sealed class ReportsController
{
    private readonly OessRepository repository;
    public ReportsController(OessRepository repository) => this.repository = repository;
    public ReportSummary Dashboard(User user)
    {
        if (user.Role is not (UserRole.Admin or UserRole.Seller or UserRole.SalesOfficer)) throw new UnauthorizedAccessException("Staff access is required.");
        return repository.GetReport();
    }
}
