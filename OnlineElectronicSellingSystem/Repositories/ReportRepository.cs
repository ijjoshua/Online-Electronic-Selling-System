using MySqlConnector;
using OnlineElectronicSellingSystem.Data;
using OnlineElectronicSellingSystem.Models;

namespace OnlineElectronicSellingSystem.Repositories;

public sealed class ReportRepository
{
    private readonly Database database;
    public ReportRepository(Database database)=>this.database=database;
    public IReadOnlyList<SalesReportRecord> Sales(DateTime from,DateTime to){using var c=database.OpenConnection();using var q=new MySqlCommand("SELECT o.order_number,u.full_name,o.total,o.payment_status,o.order_date FROM orders o JOIN users u ON u.id=o.user_id WHERE o.order_date>=@from AND o.order_date<@to ORDER BY o.order_date DESC",c);q.Parameters.AddWithValue("@from",from);q.Parameters.AddWithValue("@to",to);using var r=q.ExecuteReader();var a=new List<SalesReportRecord>();while(r.Read())a.Add(new SalesReportRecord{OrderNumber=r.GetString("order_number"),CustomerName=r.GetString("full_name"),Total=r.GetDecimal("total"),PaymentStatus=r.GetString("payment_status"),OrderDate=r.GetDateTime("order_date")});return a;}
    public IReadOnlyList<PaymentReportRecord> Payments(DateTime from,DateTime to){using var c=database.OpenConnection();using var q=new MySqlCommand("SELECT o.order_number,p.method,p.amount,p.status,p.created_at FROM payments p JOIN orders o ON o.id=p.order_id WHERE p.created_at>=@from AND p.created_at<@to ORDER BY p.created_at DESC",c);q.Parameters.AddWithValue("@from",from);q.Parameters.AddWithValue("@to",to);using var r=q.ExecuteReader();var a=new List<PaymentReportRecord>();while(r.Read())a.Add(new PaymentReportRecord{OrderNumber=r.GetString("order_number"),Method=r.GetString("method"),Amount=r.GetDecimal("amount"),Status=r.GetString("status"),CreatedAt=r.GetDateTime("created_at")});return a;}
    public IReadOnlyList<InventoryReportRecord> Inventory(){using var c=database.OpenConnection();using var q=new MySqlCommand("SELECT sku,name,stock_quantity,reorder_level FROM products WHERE is_active=1 ORDER BY stock_quantity,name",c);using var r=q.ExecuteReader();var a=new List<InventoryReportRecord>();while(r.Read())a.Add(new InventoryReportRecord{Sku=r.GetString("sku"),ProductName=r.GetString("name"),StockQuantity=r.GetInt32("stock_quantity"),ReorderLevel=r.GetInt32("reorder_level")});return a;}
}
