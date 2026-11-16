using OnlineElectronicSellingSystem.Models;
using OnlineElectronicSellingSystem.Repositories;

namespace OnlineElectronicSellingSystem.Controllers;

public sealed class SupportController
{
    private readonly SupportRepository repository;
    public SupportController(SupportRepository repository)=>this.repository=repository;
    private static void Staff(User u){if(u.Role is not(UserRole.Admin or UserRole.SupportAgent or UserRole.SalesOfficer))throw new UnauthorizedAccessException("Support access is restricted.");}
    public IReadOnlyList<SupportTicketRecord> Tickets(User u){Staff(u);return repository.Tickets();}
    public IReadOnlyList<TicketMessageRecord> Messages(User u,int ticket){Staff(u);return repository.Messages(ticket);}
    public void AddMessage(User u,int ticket,string message){Staff(u);if(string.IsNullOrWhiteSpace(message))throw new ArgumentException("Message is required.");repository.AddMessage(ticket,u.Id,message);}
    public void UpdateTicket(User u,int id,string status){Staff(u);repository.UpdateTicket(id,status);}
    public IReadOnlyList<NotificationRecord> Notifications(User u)=>repository.Notifications(u.Id);
    public void MarkNotification(User u,int id)=>repository.MarkNotification(id,u.Id);
}
