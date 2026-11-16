using System.Text.Json;
using MySqlConnector;

namespace OnlineElectronicSellingSystem.Data;

public sealed class Database
{
    private readonly string connectionString;

    public Database()
    {
        var environmentConnectionString = Environment.GetEnvironmentVariable("OESS_CONNECTION_STRING");
        if (!string.IsNullOrWhiteSpace(environmentConnectionString))
        {
            connectionString = environmentConnectionString;
            return;
        }

        var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        if (!File.Exists(path)) throw new FileNotFoundException("appsettings.json was not found. Copy appsettings.example.json to appsettings.json or set OESS_CONNECTION_STRING.", path);
        using var document = JsonDocument.Parse(File.ReadAllText(path));
        connectionString = document.RootElement.GetProperty("ConnectionStrings").GetProperty("Default").GetString()
            ?? throw new InvalidOperationException("The Default connection string is missing.");
    }

    public MySqlConnection OpenConnection()
    {
        var connection = new MySqlConnection(connectionString);
        connection.Open();
        return connection;
    }
}
