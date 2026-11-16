using MySqlConnector;
using OnlineElectronicSellingSystem.Data;
using OnlineElectronicSellingSystem.Models;

namespace OnlineElectronicSellingSystem.Repositories;

public sealed class CatalogRepository
{
    private readonly Database database;
    public CatalogRepository(Database database) => this.database = database;

    public IReadOnlyList<Brand> GetBrands()
    {
        using var connection = database.OpenConnection();
        using var command = new MySqlCommand("SELECT id,name,logo_url,description,is_active FROM brands ORDER BY name", connection);
        using var reader = command.ExecuteReader();
        var result = new List<Brand>();
        while (reader.Read()) result.Add(new Brand
        {
            Id = reader.GetInt32("id"), Name = reader.GetString("name"),
            LogoUrl = reader.IsDBNull(reader.GetOrdinal("logo_url")) ? null : reader.GetString("logo_url"),
            Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString("description"),
            IsActive = reader.GetBoolean("is_active")
        });
        return result;
    }

    public void SaveBrand(Brand brand)
    {
        using var connection = database.OpenConnection();
        using var command = new MySqlCommand(@"INSERT INTO brands(name,logo_url,description,is_active)
VALUES(@name,@logo,@description,@active)
ON DUPLICATE KEY UPDATE logo_url=@logo,description=@description,is_active=@active", connection);
        command.Parameters.AddWithValue("@name", brand.Name.Trim());
        command.Parameters.AddWithValue("@logo", (object?)brand.LogoUrl ?? DBNull.Value);
        command.Parameters.AddWithValue("@description", (object?)brand.Description ?? DBNull.Value);
        command.Parameters.AddWithValue("@active", brand.IsActive);
        command.ExecuteNonQuery();
    }

    public void SetBrandActive(int id, bool active)
    {
        using var connection = database.OpenConnection();
        using var command = new MySqlCommand("UPDATE brands SET is_active=@active WHERE id=@id", connection);
        command.Parameters.AddWithValue("@active", active); command.Parameters.AddWithValue("@id", id); command.ExecuteNonQuery();
    }

    public IReadOnlyList<Category> GetCategories()
    {
        using var connection = database.OpenConnection();
        using var command = new MySqlCommand("SELECT id,parent_id,name,description,display_order,is_active FROM categories ORDER BY display_order,name", connection);
        using var reader = command.ExecuteReader();
        var result = new List<Category>();
        while (reader.Read()) result.Add(new Category
        {
            Id = reader.GetInt32("id"), ParentId = reader.IsDBNull(reader.GetOrdinal("parent_id")) ? null : reader.GetInt32("parent_id"),
            Name = reader.GetString("name"), Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString("description"),
            DisplayOrder = reader.GetInt32("display_order"), IsActive = reader.GetBoolean("is_active")
        });
        return result;
    }

    public void SaveCategory(Category category)
    {
        using var connection = database.OpenConnection();
        using var command = new MySqlCommand(@"INSERT INTO categories(parent_id,name,description,display_order,is_active)
VALUES(@parent,@name,@description,@displayOrder,@active)
ON DUPLICATE KEY UPDATE parent_id=@parent,name=@name,description=@description,display_order=@displayOrder,is_active=@active", connection);
        command.Parameters.AddWithValue("@parent", (object?)category.ParentId ?? DBNull.Value);
        command.Parameters.AddWithValue("@name", category.Name.Trim());
        command.Parameters.AddWithValue("@description", (object?)category.Description ?? DBNull.Value);
        command.Parameters.AddWithValue("@displayOrder", category.DisplayOrder);
        command.Parameters.AddWithValue("@active", category.IsActive);
        command.ExecuteNonQuery();
    }

    public void SetCategoryActive(int id, bool active)
    {
        using var connection = database.OpenConnection();
        using var command = new MySqlCommand("UPDATE categories SET is_active=@active WHERE id=@id", connection);
        command.Parameters.AddWithValue("@active", active); command.Parameters.AddWithValue("@id", id); command.ExecuteNonQuery();
    }
}
