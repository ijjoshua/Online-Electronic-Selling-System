using OnlineElectronicSellingSystem.Models;
using OnlineElectronicSellingSystem.Repositories;

namespace OnlineElectronicSellingSystem.Controllers;

public sealed class SettingsController
{
    private readonly SettingsRepository repository;
    public SettingsController(SettingsRepository repository)=>this.repository=repository;
    private static void Admin(User u){if(u.Role!=UserRole.Admin)throw new UnauthorizedAccessException("Administrator access is required.");}
    public IReadOnlyList<StoreSettingRecord> All(User u){Admin(u);return repository.GetAll();}
    public void Save(User u,string key,string? value){Admin(u);if(string.IsNullOrWhiteSpace(key))throw new ArgumentException("Setting key is required.");repository.Save(key,value,u.Id);}
}
