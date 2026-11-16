using MySqlConnector;
using OnlineElectronicSellingSystem.Data;
using OnlineElectronicSellingSystem.Models;

namespace OnlineElectronicSellingSystem.Repositories;

public sealed class PromotionRepository
{
    private readonly Database database;
    public PromotionRepository(Database database)=>this.database=database;
    public IReadOnlyList<PromotionRecord> GetAll(){using var c=database.OpenConnection();using var q=new MySqlCommand("SELECT id,name,description,discount_type,discount_value,valid_from,valid_until,is_active FROM promotions ORDER BY valid_until DESC",c);using var r=q.ExecuteReader();var a=new List<PromotionRecord>();while(r.Read())a.Add(new PromotionRecord{Id=r.GetInt32("id"),Name=r.GetString("name"),Description=r.IsDBNull(r.GetOrdinal("description"))?null:r.GetString("description"),DiscountType=r.GetString("discount_type"),DiscountValue=r.GetDecimal("discount_value"),ValidFrom=r.GetDateTime("valid_from"),ValidUntil=r.GetDateTime("valid_until"),IsActive=r.GetBoolean("is_active")});return a;}
    public void Save(PromotionRecord p){using var c=database.OpenConnection();using var q=new MySqlCommand(@"INSERT INTO promotions(name,description,discount_type,discount_value,valid_from,valid_until,is_active) VALUES(@name,@description,@type,@value,@from,@until,@active) ON DUPLICATE KEY UPDATE description=@description,discount_type=@type,discount_value=@value,valid_from=@from,valid_until=@until,is_active=@active",c);q.Parameters.AddWithValue("@name",p.Name.Trim());q.Parameters.AddWithValue("@description",(object?)p.Description??DBNull.Value);q.Parameters.AddWithValue("@type",p.DiscountType);q.Parameters.AddWithValue("@value",p.DiscountValue);q.Parameters.AddWithValue("@from",p.ValidFrom);q.Parameters.AddWithValue("@until",p.ValidUntil);q.Parameters.AddWithValue("@active",p.IsActive);q.ExecuteNonQuery();}
}
