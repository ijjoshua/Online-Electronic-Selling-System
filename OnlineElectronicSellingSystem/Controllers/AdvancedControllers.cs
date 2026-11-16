using OnlineElectronicSellingSystem.Models;
using OnlineElectronicSellingSystem.Repositories;

namespace OnlineElectronicSellingSystem.Controllers;

public sealed class AdvancedController
{
    private readonly AdvancedRepository repository;
    public AdvancedController(AdvancedRepository repository) => this.repository = repository;

    private static void RequireStaff(User user)
    {
        if (user.Role is not (UserRole.Admin or UserRole.Seller or UserRole.SalesOfficer or UserRole.SupportAgent or UserRole.DeliveryAgent)) throw new UnauthorizedAccessException("Staff access is required.");
    }

    private static void RequireAdmin(User user)
    {
        if (user.Role != UserRole.Admin) throw new UnauthorizedAccessException("Administrator access is required.");
    }

    public IReadOnlyList<CustomerRecord> Customers(User user) { RequireAdmin(user); return repository.GetCustomers(); }
    public void SetCustomerStatus(User user, int customerId, string status) { RequireAdmin(user); repository.SetCustomerStatus(customerId, status, user.Id); }
    public IReadOnlyList<SupplierRecord> Suppliers(User user) { RequireStaff(user); return repository.GetSuppliers(); }
    public void SaveSupplier(User user, SupplierRecord supplier) { RequireStaff(user); repository.SaveSupplier(supplier, user.Id); }
    public IReadOnlyList<CouponRecord> Coupons(User user) { RequireAdmin(user); return repository.GetCoupons(); }
    public void SaveCoupon(User user, CouponRecord coupon) { RequireAdmin(user); repository.SaveCoupon(coupon, user.Id); }
    public IReadOnlyList<SupportTicketRecord> Tickets(User user) { RequireStaff(user); return repository.GetTickets(); }
    public void UpdateTicket(User user, int ticketId, string status) { RequireStaff(user); repository.UpdateTicketStatus(ticketId, status, user.Id); }
    public IReadOnlyList<ReturnRecord> Returns(User user) { RequireStaff(user); return repository.GetReturns(); }
    public void UpdateReturn(User user, int returnId, string status) { RequireStaff(user); repository.UpdateReturnStatus(returnId, status, user.Id); }
    public IReadOnlyList<WarrantyClaimRecord> WarrantyClaims(User user) { RequireStaff(user); return repository.GetWarrantyClaims(); }
    public void UpdateWarranty(User user, int claimId, string status) { RequireStaff(user); repository.UpdateWarrantyStatus(claimId, status, user.Id); }
    public IReadOnlyList<AuditRecord> AuditLogs(User user) { RequireAdmin(user); return repository.GetAuditLogs(); }
}
