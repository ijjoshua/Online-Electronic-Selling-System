using OnlineElectronicSellingSystem.Models;
using OnlineElectronicSellingSystem.Repositories;

namespace OnlineElectronicSellingSystem.Controllers;

public sealed class UserManagementController
{
    private readonly UserManagementRepository repository;
    public UserManagementController(UserManagementRepository repository) => this.repository = repository;

    private static void RequireAdmin(User user)
    {
        if (user.Role != UserRole.Admin) throw new UnauthorizedAccessException("Administrator access is required.");
    }

    public IReadOnlyList<UserRecord> Search(User actor, string? search) { RequireAdmin(actor); return repository.Search(search); }
    public void Add(User actor, UserRecord user, string password) { RequireAdmin(actor); Validate(user, password, true); repository.Add(user, password); }
    public void Update(User actor, UserRecord user, string? password) { RequireAdmin(actor); Validate(user, password, false); repository.Update(user, password); }
    public void UpdateRole(User actor, int userId, UserRole role) { RequireAdmin(actor); repository.UpdateRole(userId, role.ToString()); }

    private static void Validate(UserRecord user, string? password, bool requirePassword)
    {
        if (string.IsNullOrWhiteSpace(user.FullName) || string.IsNullOrWhiteSpace(user.Email)) throw new ArgumentException("Full name and email are required.");
        if (requirePassword && string.IsNullOrWhiteSpace(password)) throw new ArgumentException("A password is required for a new user.");
        if (!string.IsNullOrWhiteSpace(password) && password.Length < 8) throw new ArgumentException("Password must contain at least 8 characters.");
        if (!Enum.TryParse<UserRole>(user.Role, true, out _)) throw new ArgumentException("Select a valid role.");
    }
}
