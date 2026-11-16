using OnlineElectronicSellingSystem.Models;
using OnlineElectronicSellingSystem.Repositories;

namespace OnlineElectronicSellingSystem.Controllers;

public sealed class PaymentController
{
    private readonly PaymentRepository repository;
    public PaymentController(PaymentRepository repository) => this.repository = repository;
    private static void RequireStaff(User user) { if (user.Role is not (UserRole.Admin or UserRole.SalesOfficer)) throw new UnauthorizedAccessException("Payment access is restricted to administrators and sales officers."); }
    public IReadOnlyList<PaymentRecord> Payments(User user) { RequireStaff(user); return repository.GetPayments(); }
    public void UpdateStatus(User user, int id, string status, string? reference) { RequireStaff(user); repository.UpdateStatus(id,status,reference); }
    public IReadOnlyList<InvoiceRecord> Invoices(User user) { RequireStaff(user); return repository.GetInvoices(); }
    public void CreateInvoice(User user, int orderId, string number) { RequireStaff(user); repository.CreateInvoice(orderId,number); }
}
