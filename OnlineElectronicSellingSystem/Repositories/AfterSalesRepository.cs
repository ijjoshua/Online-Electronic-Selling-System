using MySqlConnector;
using OnlineElectronicSellingSystem.Data;
using OnlineElectronicSellingSystem.Models;

namespace OnlineElectronicSellingSystem.Repositories;

public sealed class AfterSalesRepository
{
    private readonly Database database;
    public AfterSalesRepository(Database database) => this.database = database;

    public IReadOnlyList<ReturnRecord> Returns()
    {
        using var c=database.OpenConnection(); using var cmd=new MySqlCommand(@"SELECT r.id,r.rma_number,o.order_number,p.name,u.full_name,r.reason,r.status,r.requested_at FROM returns r JOIN order_items oi ON oi.id=r.order_item_id JOIN orders o ON o.id=oi.order_id JOIN products p ON p.id=oi.product_id JOIN users u ON u.id=r.user_id ORDER BY r.requested_at DESC",c); using var rd=cmd.ExecuteReader(); var a=new List<ReturnRecord>(); while(rd.Read()) a.Add(new ReturnRecord{Id=rd.GetInt32("id"),RmaNumber=rd.IsDBNull(rd.GetOrdinal("rma_number"))?null:rd.GetString("rma_number"),OrderNumber=rd.GetString("order_number"),ProductName=rd.GetString("name"),CustomerName=rd.GetString("full_name"),Reason=rd.GetString("reason"),Status=rd.GetString("status"),RequestedAt=rd.GetDateTime("requested_at")}); return a;
    }
    public IReadOnlyList<WarrantyRecord> Warranties()
    {
        using var c=database.OpenConnection(); using var cmd=new MySqlCommand(@"SELECT w.id,o.order_number,p.name,w.serial_number,w.start_date,w.end_date,w.status FROM warranties w JOIN order_items oi ON oi.id=w.order_item_id JOIN orders o ON o.id=oi.order_id JOIN products p ON p.id=oi.product_id ORDER BY w.end_date",c); using var rd=cmd.ExecuteReader(); var a=new List<WarrantyRecord>(); while(rd.Read()) a.Add(new WarrantyRecord{Id=rd.GetInt32("id"),OrderNumber=rd.GetString("order_number"),ProductName=rd.GetString("name"),SerialNumber=rd.IsDBNull(rd.GetOrdinal("serial_number"))?null:rd.GetString("serial_number"),StartDate=rd.GetDateTime("start_date"),EndDate=rd.GetDateTime("end_date"),Status=rd.GetString("status")}); return a;
    }
    public IReadOnlyList<WarrantyClaimRecord> Claims()
    {
        using var c=database.OpenConnection(); using var cmd=new MySqlCommand(@"SELECT wc.id,wc.warranty_id,u.full_name,p.name,wc.description,wc.status,wc.created_at FROM warranty_claims wc JOIN warranties w ON w.id=wc.warranty_id JOIN order_items oi ON oi.id=w.order_item_id JOIN products p ON p.id=oi.product_id JOIN users u ON u.id=wc.user_id ORDER BY wc.created_at DESC",c); using var rd=cmd.ExecuteReader(); var a=new List<WarrantyClaimRecord>(); while(rd.Read()) a.Add(new WarrantyClaimRecord{Id=rd.GetInt32("id"),WarrantyId=rd.GetInt32("warranty_id"),CustomerName=rd.GetString("full_name"),ProductName=rd.GetString("name"),Description=rd.GetString("description"),Status=rd.GetString("status"),CreatedAt=rd.GetDateTime("created_at")}); return a;
    }
    public void UpdateReturn(int id,string status){Update("UPDATE returns SET status=@status,resolved_at=IF(@status IN ('Rejected','Completed'),CURRENT_TIMESTAMP,NULL) WHERE id=@id",id,status);}
    public void UpdateWarranty(int id,string status){Update("UPDATE warranties SET status=@status WHERE id=@id",id,status);}
    public void UpdateClaim(int id,string status,string? resolution){using var c=database.OpenConnection();using var cmd=new MySqlCommand("UPDATE warranty_claims SET status=@status,resolution=@resolution,resolved_at=IF(@status IN ('Resolved','Rejected'),CURRENT_TIMESTAMP,NULL) WHERE id=@id",c);cmd.Parameters.AddWithValue("@status",status);cmd.Parameters.AddWithValue("@resolution",(object?)resolution??DBNull.Value);cmd.Parameters.AddWithValue("@id",id);cmd.ExecuteNonQuery();}
    private void Update(string sql,int id,string status){using var c=database.OpenConnection();using var cmd=new MySqlCommand(sql,c);cmd.Parameters.AddWithValue("@status",status);cmd.Parameters.AddWithValue("@id",id);cmd.ExecuteNonQuery();}
}
