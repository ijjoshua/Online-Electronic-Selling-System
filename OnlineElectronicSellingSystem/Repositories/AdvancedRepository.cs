using MySqlConnector;
using OnlineElectronicSellingSystem.Data;
using OnlineElectronicSellingSystem.Models;

namespace OnlineElectronicSellingSystem.Repositories;

public sealed class AdvancedRepository
{
    private readonly Database database;
    public AdvancedRepository(Database database) => this.database = database;

    public IReadOnlyList<CustomerRecord> GetCustomers()
    {
        using var connection = database.OpenConnection();
        using var command = new MySqlCommand("SELECT id,full_name,email,phone,status,created_at FROM users WHERE role='Customer' ORDER BY created_at DESC", connection);
        using var reader = command.ExecuteReader(); var result = new List<CustomerRecord>();
        while (reader.Read()) result.Add(new CustomerRecord { Id=reader.GetInt32("id"), FullName=reader.GetString("full_name"), Email=reader.GetString("email"), Phone=reader.IsDBNull(reader.GetOrdinal("phone")) ? null : reader.GetString("phone"), Status=reader.GetString("status"), CreatedAt=reader.GetDateTime("created_at") });
        return result;
    }

    public void SetCustomerStatus(int customerId, string status, int actorId) => ExecuteAudit("UPDATE users SET status=@status WHERE id=@id AND role='Customer'", status, customerId, actorId, "CustomerStatusChanged", "users");

    public IReadOnlyList<SupplierRecord> GetSuppliers()
    {
        using var connection = database.OpenConnection(); using var command = new MySqlCommand("SELECT id,name,contact_name,email,phone,is_active FROM suppliers ORDER BY name", connection);
        using var reader=command.ExecuteReader(); var result=new List<SupplierRecord>();
        while(reader.Read()) result.Add(new SupplierRecord { Id=reader.GetInt32("id"), Name=reader.GetString("name"), ContactName=reader.IsDBNull(reader.GetOrdinal("contact_name"))?null:reader.GetString("contact_name"), Email=reader.IsDBNull(reader.GetOrdinal("email"))?null:reader.GetString("email"), Phone=reader.IsDBNull(reader.GetOrdinal("phone"))?null:reader.GetString("phone"), IsActive=reader.GetBoolean("is_active") }); return result;
    }

    public void SaveSupplier(SupplierRecord supplier, int actorId)
    {
        using var connection=database.OpenConnection(); using var command=new MySqlCommand("INSERT INTO suppliers(name,contact_name,email,phone,is_active) VALUES(@name,@contact,@email,@phone,1) ON DUPLICATE KEY UPDATE name=@name,contact_name=@contact,email=@email,phone=@phone,is_active=1",connection);
        command.Parameters.AddWithValue("@name", supplier.Name.Trim()); command.Parameters.AddWithValue("@contact", (object?)supplier.ContactName ?? DBNull.Value); command.Parameters.AddWithValue("@email", (object?)supplier.Email ?? DBNull.Value); command.Parameters.AddWithValue("@phone", (object?)supplier.Phone ?? DBNull.Value); command.ExecuteNonQuery(); WriteAudit(connection, actorId, "SupplierSaved", "suppliers", (int)command.LastInsertedId);
    }

    public IReadOnlyList<CouponRecord> GetCoupons()
    {
        using var connection=database.OpenConnection(); using var command=new MySqlCommand("SELECT id,code,discount_type,value,minimum_order_amount,valid_from,valid_until,is_active FROM coupons ORDER BY valid_until DESC",connection); using var reader=command.ExecuteReader(); var result=new List<CouponRecord>();
        while(reader.Read()) result.Add(new CouponRecord { Id=reader.GetInt32("id"), Code=reader.GetString("code"), DiscountType=reader.GetString("discount_type"), Value=reader.GetDecimal("value"), MinimumOrderAmount=reader.GetDecimal("minimum_order_amount"), ValidFrom=reader.GetDateTime("valid_from"), ValidUntil=reader.GetDateTime("valid_until"), IsActive=reader.GetBoolean("is_active") }); return result;
    }

    public void SaveCoupon(CouponRecord coupon, int actorId)
    {
        using var connection=database.OpenConnection(); using var command=new MySqlCommand("INSERT INTO coupons(code,discount_type,value,minimum_order_amount,valid_from,valid_until,is_active) VALUES(@code,@type,@value,@minimum,@from,@until,1) ON DUPLICATE KEY UPDATE discount_type=@type,value=@value,minimum_order_amount=@minimum,valid_from=@from,valid_until=@until,is_active=1",connection);
        command.Parameters.AddWithValue("@code",coupon.Code.Trim().ToUpperInvariant()); command.Parameters.AddWithValue("@type",coupon.DiscountType); command.Parameters.AddWithValue("@value",coupon.Value); command.Parameters.AddWithValue("@minimum",coupon.MinimumOrderAmount); command.Parameters.AddWithValue("@from",coupon.ValidFrom); command.Parameters.AddWithValue("@until",coupon.ValidUntil); command.ExecuteNonQuery(); WriteAudit(connection,actorId,"CouponSaved","coupons",(int)command.LastInsertedId);
    }

    public IReadOnlyList<SupportTicketRecord> GetTickets()
    {
        using var connection=database.OpenConnection(); using var command=new MySqlCommand("SELECT t.id,u.full_name,t.subject,t.category,t.status,t.created_at FROM support_tickets t JOIN users u ON u.id=t.user_id ORDER BY t.created_at DESC",connection); using var reader=command.ExecuteReader(); var result=new List<SupportTicketRecord>();
        while(reader.Read()) result.Add(new SupportTicketRecord { Id=reader.GetInt32("id"),CustomerName=reader.GetString("full_name"),Subject=reader.GetString("subject"),Category=reader.GetString("category"),Status=reader.GetString("status"),CreatedAt=reader.GetDateTime("created_at") }); return result;
    }

    public void UpdateTicketStatus(int ticketId, string status, int actorId) => ExecuteAudit("UPDATE support_tickets SET status=@status, closed_at=IF(@status='Closed',CURRENT_TIMESTAMP,NULL) WHERE id=@id", status, ticketId, actorId, "TicketStatusChanged", "support_tickets");

    public IReadOnlyList<ReturnRecord> GetReturns()
    {
        using var connection=database.OpenConnection(); using var command=new MySqlCommand("SELECT r.id,COALESCE(r.rma_number,''),u.full_name,r.reason,r.status,r.requested_at FROM returns r JOIN users u ON u.id=r.user_id ORDER BY r.requested_at DESC",connection); using var reader=command.ExecuteReader(); var result=new List<ReturnRecord>();
        while(reader.Read()) result.Add(new ReturnRecord { Id=reader.GetInt32("id"),RmaNumber=reader.GetString(2),CustomerName=reader.GetString("full_name"),Reason=reader.GetString("reason"),Status=reader.GetString("status"),RequestedAt=reader.GetDateTime("requested_at") }); return result;
    }

    public void UpdateReturnStatus(int returnId, string status, int actorId)
    {
        using var connection=database.OpenConnection(); using var command=new MySqlCommand("UPDATE returns SET status=@status,rma_number=IF(@status='Approved',COALESCE(rma_number,CONCAT('RMA-',id)),rma_number),resolved_at=IF(@status IN ('Rejected','Completed'),CURRENT_TIMESTAMP,NULL) WHERE id=@id",connection); command.Parameters.AddWithValue("@status",status); command.Parameters.AddWithValue("@id",returnId); command.ExecuteNonQuery(); WriteAudit(connection,actorId,"ReturnStatusChanged","returns",returnId);
    }

    public IReadOnlyList<WarrantyClaimRecord> GetWarrantyClaims()
    {
        using var connection=database.OpenConnection(); using var command=new MySqlCommand("SELECT c.id,u.full_name,c.description,c.status,c.created_at FROM warranty_claims c JOIN users u ON u.id=c.user_id ORDER BY c.created_at DESC",connection); using var reader=command.ExecuteReader(); var result=new List<WarrantyClaimRecord>();
        while(reader.Read()) result.Add(new WarrantyClaimRecord { Id=reader.GetInt32("id"),CustomerName=reader.GetString("full_name"),Description=reader.GetString("description"),Status=reader.GetString("status"),CreatedAt=reader.GetDateTime("created_at") }); return result;
    }

    public void UpdateWarrantyStatus(int claimId, string status, int actorId) => ExecuteAudit("UPDATE warranty_claims SET status=@status,resolved_at=IF(@status IN ('Rejected','Completed'),CURRENT_TIMESTAMP,NULL) WHERE id=@id", status, claimId, actorId, "WarrantyStatusChanged", "warranty_claims");

    public IReadOnlyList<AuditRecord> GetAuditLogs()
    {
        using var connection=database.OpenConnection(); using var command=new MySqlCommand("SELECT a.id,u.full_name,a.action,a.entity_type,a.entity_id,a.created_at FROM audit_logs a LEFT JOIN users u ON u.id=a.user_id ORDER BY a.created_at DESC LIMIT 500",connection); using var reader=command.ExecuteReader(); var result=new List<AuditRecord>();
        while(reader.Read()) result.Add(new AuditRecord { Id=reader.GetInt64("id"),UserName=reader.IsDBNull(reader.GetOrdinal("full_name"))?null:reader.GetString("full_name"),Action=reader.GetString("action"),EntityType=reader.IsDBNull(reader.GetOrdinal("entity_type"))?null:reader.GetString("entity_type"),EntityId=reader.IsDBNull(reader.GetOrdinal("entity_id"))?null:reader.GetInt32("entity_id"),CreatedAt=reader.GetDateTime("created_at") }); return result;
    }

    private void ExecuteAudit(string sql,string status,int entityId,int actorId,string action,string entityType)
    { using var connection=database.OpenConnection(); using var command=new MySqlCommand(sql,connection); command.Parameters.AddWithValue("@status",status); command.Parameters.AddWithValue("@id",entityId); command.ExecuteNonQuery(); WriteAudit(connection,actorId,action,entityType,entityId); }
    private static void WriteAudit(MySqlConnection connection,int actorId,string action,string entityType,int entityId)
    { using var command=new MySqlCommand("INSERT INTO audit_logs(user_id,action,entity_type,entity_id) VALUES(@user,@action,@type,@id)",connection); command.Parameters.AddWithValue("@user",actorId); command.Parameters.AddWithValue("@action",action); command.Parameters.AddWithValue("@type",entityType); command.Parameters.AddWithValue("@id",entityId); command.ExecuteNonQuery(); }
}

