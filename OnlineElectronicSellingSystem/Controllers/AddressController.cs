using OnlineElectronicSellingSystem.Models;
using OnlineElectronicSellingSystem.Repositories;

namespace OnlineElectronicSellingSystem.Controllers;

public sealed class AddressController
{
    private readonly AddressRepository repository;
    public AddressController(AddressRepository repository)=>this.repository=repository;
    public IReadOnlyList<AddressRecord> Addresses(User user){return repository.Get(user.Role==UserRole.Admin?null:user.Id);}
    public void Save(User actor,AddressRecord address){if(actor.Role!=UserRole.Admin&&actor.Id!=address.UserId)throw new UnauthorizedAccessException("You can only manage your own addresses.");if(string.IsNullOrWhiteSpace(address.Recipient)||string.IsNullOrWhiteSpace(address.Street)||string.IsNullOrWhiteSpace(address.City)||string.IsNullOrWhiteSpace(address.Country))throw new ArgumentException("Recipient, street, city, and country are required.");repository.Save(address);}
}
