using MySqlConnector;
using OnlineElectronicSellingSystem.Data;
using OnlineElectronicSellingSystem.Models;

namespace OnlineElectronicSellingSystem.Repositories;

public sealed class AddressRepository
{
    private readonly Database database;
    public AddressRepository(Database database)=>this.database=database;
    public IReadOnlyList<AddressRecord> Get(int? userId=null){using var c=database.OpenConnection();using var q=new MySqlCommand(@"SELECT a.id,a.user_id,u.full_name,a.recipient,a.street,a.city,a.region,a.country,a.phone,a.is_default,a.created_at FROM addresses a JOIN users u ON u.id=a.user_id WHERE (@user=0 OR a.user_id=@user) ORDER BY a.is_default DESC,a.created_at DESC",c);q.Parameters.AddWithValue("@user",userId??0);using var r=q.ExecuteReader();var a=new List<AddressRecord>();while(r.Read())a.Add(new AddressRecord{Id=r.GetInt32("id"),UserId=r.GetInt32("user_id"),UserName=r.GetString("full_name"),Recipient=r.GetString("recipient"),Street=r.GetString("street"),City=r.GetString("city"),Region=r.IsDBNull(r.GetOrdinal("region"))?null:r.GetString("region"),Country=r.GetString("country"),Phone=r.IsDBNull(r.GetOrdinal("phone"))?null:r.GetString("phone"),IsDefault=r.GetBoolean("is_default"),CreatedAt=r.GetDateTime("created_at")});return a;}
    public void Save(AddressRecord a){using var c=database.OpenConnection();using var t=c.BeginTransaction();try{if(a.IsDefault){using var clear=new MySqlCommand("UPDATE addresses SET is_default=FALSE WHERE user_id=@user",c,t);clear.Parameters.AddWithValue("@user",a.UserId);clear.ExecuteNonQuery();}using var q=new MySqlCommand("INSERT INTO addresses(user_id,recipient,street,city,region,country,phone,is_default) VALUES(@user,@recipient,@street,@city,@region,@country,@phone,@default)",c,t);q.Parameters.AddWithValue("@user",a.UserId);q.Parameters.AddWithValue("@recipient",a.Recipient.Trim());q.Parameters.AddWithValue("@street",a.Street.Trim());q.Parameters.AddWithValue("@city",a.City.Trim());q.Parameters.AddWithValue("@region",(object?)a.Region??DBNull.Value);q.Parameters.AddWithValue("@country",a.Country.Trim());q.Parameters.AddWithValue("@phone",(object?)a.Phone??DBNull.Value);q.Parameters.AddWithValue("@default",a.IsDefault);q.ExecuteNonQuery();t.Commit();}catch{t.Rollback();throw;}}
}
