using MySqlConnector;
using OnlineElectronicSellingSystem.Data;
using OnlineElectronicSellingSystem.Models;

namespace OnlineElectronicSellingSystem.Repositories;

public sealed class SettingsRepository
{
    private readonly Database database;
    public SettingsRepository(Database database)=>this.database=database;
    public IReadOnlyList<StoreSettingRecord> GetAll(){using var c=database.OpenConnection();using var cmd=new MySqlCommand("SELECT setting_key,setting_value,updated_by,updated_at FROM store_settings ORDER BY setting_key",c);using var r=cmd.ExecuteReader();var a=new List<StoreSettingRecord>();while(r.Read())a.Add(new StoreSettingRecord{SettingKey=r.GetString("setting_key"),SettingValue=r.IsDBNull(r.GetOrdinal("setting_value"))?null:r.GetString("setting_value"),UpdatedBy=r.IsDBNull(r.GetOrdinal("updated_by"))?null:r.GetInt32("updated_by"),UpdatedAt=r.GetDateTime("updated_at")});return a;}
    public void Save(string key,string? value,int userId){using var c=database.OpenConnection();using var cmd=new MySqlCommand("INSERT INTO store_settings(setting_key,setting_value,updated_by) VALUES(@key,@value,@user) ON DUPLICATE KEY UPDATE setting_value=@value,updated_by=@user",c);cmd.Parameters.AddWithValue("@key",key.Trim());cmd.Parameters.AddWithValue("@value",(object?)value??DBNull.Value);cmd.Parameters.AddWithValue("@user",userId);cmd.ExecuteNonQuery();}
}
