using MySqlConnector;

namespace OnlineElectronicSellingSystem.Data;

public sealed class DevelopmentAdminProvisioner
{
    private readonly Database database;
    public DevelopmentAdminProvisioner(Database database) => this.database = database;

    public void EnsureAdmin()
    {
        using var connection = database.OpenConnection();
        using var command = new MySqlCommand(@"INSERT INTO users(full_name,email,password_hash,role,status)
VALUES(@name,@email,@password,'Admin','Active')
ON DUPLICATE KEY UPDATE full_name=@name,password_hash=@password,role='Admin',status='Active'", connection);
        command.Parameters.AddWithValue("@name", "System Administrator");
        command.Parameters.AddWithValue("@email", "admin@oess.local");
        command.Parameters.AddWithValue("@password", PasswordHasher.Hash("admin"));
        command.ExecuteNonQuery();
    }
}
