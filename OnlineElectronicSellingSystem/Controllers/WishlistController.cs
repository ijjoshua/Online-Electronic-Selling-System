using OnlineElectronicSellingSystem.Models;
using OnlineElectronicSellingSystem.Repositories;

namespace OnlineElectronicSellingSystem.Controllers;

public sealed class WishlistController
{
    private readonly WishlistRepository repository;
    public WishlistController(WishlistRepository repository) => this.repository = repository;
    public IReadOnlyList<WishlistItemRecord> Items(User user) => repository.GetItems(user.Id);
    public void Add(User user, int productId) => repository.Add(user.Id, productId);
    public void Remove(User user, int productId) => repository.Remove(user.Id, productId);
}
