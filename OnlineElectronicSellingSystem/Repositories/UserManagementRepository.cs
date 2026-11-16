using MySqlConnector;
using OnlineElectronicSellingSystem.Data;
using OnlineElectronicSellingSystem.Models;

namespace OnlineElectronicSellingSystem.Repositories;

public sealed class UserManagementRepository
{
    private readonly Database database;
    public UserManagementRepository(Database database) => this.database = database;

    public IReadOnlyList<UserRecord> Search(string? search)
    {
        using var connection = database.OpenConnection();
        using var command = new MySqlCommand(@"SELECT id,full_name,email,phone,role,status,created_at
FROM users WHERE @search='' OR full_name LIKE @like OR email LIKE @like OR phone LIKE @like
ORDER BY created_at DESC", connection);
        var value = search?.Trim() ?? "";
        command.Parameters.AddWithValue("@search", value); command.Parameters.AddWithValue("@like", $"%{value}%");
        using var reader = command.ExecuteReader();
        var result = new List<UserRecord>();
        while (reader.Read()) result.Add(new UserRecord
        {
            Id = reader.GetInt32("id"), FullName = reader.GetString("full_name"), Email = reader.GetString("email"),
            Phone = reader.IsDBNull(reader.GetOrdinal("phone")) ? null : reader.GetString("phone"), Role = reader.GetString("role"),
            Status = reader.GetString("status"), CreatedAt = reader.GetDateTime("created_at")
        });
        return result;
    }

    public void Add(UserRecord user, string password)
    {
        using var connection = database.OpenConnection();
        using var command = new MySqlCommand("INSERT INTO users(full_name,email,phone,password_hash,role,status) VALUES(@name,@email,@phone,@password,@role,@status)", connection);
        AddParameters(command, user, password); command.ExecuteNonQuery();
    }

    public void Update(UserRecord user, string? password)
    {
        using var connection = database.OpenConnection();
        using var command = string.IsNullOrWhiteSpace(password)
            ? new MySqlCommand("UPDATE users SET full_name=@name,email=@email,phone=@phone,role=@role,status=@status WHERE id=@id", connection)
            : new MySqlCommand("UPDATE users SET full_name=@name,email=@email,phone=@phone,password_hash=@password,role=@role,status=@status WHERE id=@id", connection);
        command.Parameters.AddWithValue("@id", user.Id); command.Parameters.AddWithValue("@name", user.FullName.Trim()); command.Parameters.AddWithValue("@email", user.Email.Trim().ToLowerInvariant());
        command.Parameters.AddWithValue("@phone", OptionalValue(user.Phone)); command.Parameters.AddWithValue("@role", user.Role); command.Parameters.AddWithValue("@status", user.Status);
        if (!string.IsNullOrWhiteSpace(password)) command.Parameters.AddWithValue("@password", PasswordHasher.Hash(password));
        if (command.ExecuteNonQuery() == 0) throw new InvalidOperationException("The selected user could not be updated.");
    }

    public void UpdateRole(int userId, string role)
    {
        using var connection = database.OpenConnection();
        using var command = new MySqlCommand("UPDATE users SET role=@role WHERE id=@id", connection);
        command.Parameters.AddWithValue("@id", userId);
        command.Parameters.AddWithValue("@role", role);
        if (command.ExecuteNonQuery() == 0) throw new InvalidOperationException("The selected user could not be found.");
    }

    private static void AddParameters(MySqlCommand command, UserRecord user, string password)
    {
        command.Parameters.AddWithValue("@name", user.FullName.Trim()); command.Parameters.AddWithValue("@email", user.Email.Trim().ToLowerInvariant());
        command.Parameters.AddWithValue("@phone", OptionalValue(user.Phone)); command.Parameters.AddWithValue("@password", PasswordHasher.Hash(password));
        command.Parameters.AddWithValue("@role", user.Role); command.Parameters.AddWithValue("@status", user.Status);
    }

    private static object OptionalValue(string? value) => string.IsNullOrWhiteSpace(value) ? DBNull.Value : value.Trim();
}
