using OnlineElectronicSellingSystem.Models;
using OnlineElectronicSellingSystem.Repositories;

namespace OnlineElectronicSellingSystem.Controllers;

public sealed class ReportManagementController
{
    private readonly ReportRepository repository;
    public ReportManagementController(ReportRepository repository)=>this.repository=repository;
    private static void Staff(User u){if(u.Role is not(UserRole.Admin or UserRole.Seller or UserRole.SalesOfficer))throw new UnauthorizedAccessException("Report access is restricted.");}
    public IReadOnlyList<SalesReportRecord> Sales(User u,DateTime from,DateTime to){Staff(u);return repository.Sales(from,to);}
    public IReadOnlyList<PaymentReportRecord> Payments(User u,DateTime from,DateTime to){Staff(u);return repository.Payments(from,to);}
    public IReadOnlyList<InventoryReportRecord> Inventory(User u){Staff(u);return repository.Inventory();}
}
