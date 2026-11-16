using MySqlConnector;
using OnlineElectronicSellingSystem.Data;
using OnlineElectronicSellingSystem.Models;

namespace OnlineElectronicSellingSystem.Repositories;

public sealed class PaymentRepository
{
    private readonly Database database;
    public PaymentRepository(Database database) => this.database = database;

    public IReadOnlyList<PaymentRecord> GetPayments()
    {
        using var connection = database.OpenConnection();
        using var command = new MySqlCommand(@"SELECT p.id,p.order_id,o.order_number,p.method,p.amount,p.status,p.reference,p.paid_at,p.created_at
FROM payments p JOIN orders o ON o.id=p.order_id ORDER BY p.created_at DESC", connection);
        using var reader = command.ExecuteReader(); var result = new List<PaymentRecord>();
        while (reader.Read()) result.Add(new PaymentRecord { Id=reader.GetInt32("id"), OrderId=reader.GetInt32("order_id"), OrderNumber=reader.GetString("order_number"), Method=reader.GetString("method"), Amount=reader.GetDecimal("amount"), Status=reader.GetString("status"), Reference=reader.IsDBNull(reader.GetOrdinal("reference"))?null:reader.GetString("reference"), PaidAt=reader.IsDBNull(reader.GetOrdinal("paid_at"))?null:reader.GetDateTime("paid_at"), CreatedAt=reader.GetDateTime("created_at") });
        return result;
    }

    public void UpdateStatus(int id, string status, string? reference)
    {
        using var connection = database.OpenConnection();
        using var command = new MySqlCommand("UPDATE payments SET status=@status,reference=@reference,paid_at=IF(@status='Confirmed',COALESCE(paid_at,CURRENT_TIMESTAMP),NULL) WHERE id=@id", connection);
        command.Parameters.AddWithValue("@status", status); command.Parameters.AddWithValue("@reference", (object?)reference ?? DBNull.Value); command.Parameters.AddWithValue("@id", id); command.ExecuteNonQuery();
    }

    public IReadOnlyList<InvoiceRecord> GetInvoices()
    {
        using var connection = database.OpenConnection();
        using var command = new MySqlCommand("SELECT i.id,i.order_id,o.order_number,i.invoice_number,i.pdf_url,i.issued_at FROM invoices i JOIN orders o ON o.id=i.order_id ORDER BY i.issued_at DESC", connection);
        using var reader = command.ExecuteReader(); var result = new List<InvoiceRecord>();
        while (reader.Read()) result.Add(new InvoiceRecord { Id=reader.GetInt32("id"), OrderId=reader.GetInt32("order_id"), OrderNumber=reader.GetString("order_number"), InvoiceNumber=reader.GetString("invoice_number"), PdfUrl=reader.IsDBNull(reader.GetOrdinal("pdf_url"))?null:reader.GetString("pdf_url"), IssuedAt=reader.GetDateTime("issued_at") });
        return result;
    }

    public void CreateInvoice(int orderId, string invoiceNumber)
    {
        using var connection = database.OpenConnection();
        using var command = new MySqlCommand("INSERT INTO invoices(order_id,invoice_number) VALUES(@order,@number)", connection);
        command.Parameters.AddWithValue("@order", orderId); command.Parameters.AddWithValue("@number", invoiceNumber.Trim()); command.ExecuteNonQuery();
    }
}
