using MySqlConnector;
using OnlineElectronicSellingSystem.Data;
using OnlineElectronicSellingSystem.Models;

namespace OnlineElectronicSellingSystem.Repositories;

public sealed class CouponRepository
{
    private readonly Database database;
    public CouponRepository(Database database)=>this.database=database;
    public IReadOnlyList<CouponManagementRecord> GetAll(){using var c=database.OpenConnection();using var q=new MySqlCommand("SELECT id,code,discount_type,value,minimum_order_amount,usage_limit,usage_count,valid_from,valid_until,is_active FROM coupons ORDER BY valid_until DESC",c);using var r=q.ExecuteReader();var a=new List<CouponManagementRecord>();while(r.Read())a.Add(new CouponManagementRecord{Id=r.GetInt32("id"),Code=r.GetString("code"),DiscountType=r.GetString("discount_type"),Value=r.GetDecimal("value"),MinimumOrderAmount=r.GetDecimal("minimum_order_amount"),UsageLimit=r.IsDBNull(r.GetOrdinal("usage_limit"))?null:r.GetInt32("usage_limit"),UsageCount=r.GetInt32("usage_count"),ValidFrom=r.GetDateTime("valid_from"),ValidUntil=r.GetDateTime("valid_until"),IsActive=r.GetBoolean("is_active")});return a;}
    public void Save(CouponManagementRecord cpn){using var c=database.OpenConnection();using var q=new MySqlCommand(@"INSERT INTO coupons(code,discount_type,value,minimum_order_amount,usage_limit,valid_from,valid_until,is_active) VALUES(@code,@type,@value,@minimum,@limit,@from,@until,@active) ON DUPLICATE KEY UPDATE discount_type=@type,value=@value,minimum_order_amount=@minimum,usage_limit=@limit,valid_from=@from,valid_until=@until,is_active=@active",c);q.Parameters.AddWithValue("@code",cpn.Code.Trim().ToUpperInvariant());q.Parameters.AddWithValue("@type",cpn.DiscountType);q.Parameters.AddWithValue("@value",cpn.Value);q.Parameters.AddWithValue("@minimum",cpn.MinimumOrderAmount);q.Parameters.AddWithValue("@limit",(object?)cpn.UsageLimit??DBNull.Value);q.Parameters.AddWithValue("@from",cpn.ValidFrom);q.Parameters.AddWithValue("@until",cpn.ValidUntil);q.Parameters.AddWithValue("@active",cpn.IsActive);q.ExecuteNonQuery();}
}
