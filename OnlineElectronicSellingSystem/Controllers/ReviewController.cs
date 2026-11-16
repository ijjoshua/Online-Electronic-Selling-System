using OnlineElectronicSellingSystem.Models;
using OnlineElectronicSellingSystem.Repositories;

namespace OnlineElectronicSellingSystem.Controllers;

public sealed class ReviewController
{
    private readonly ReviewRepository repository;
    public ReviewController(ReviewRepository repository)=>this.repository=repository;
    private static void Staff(User u){if(u.Role is not(UserRole.Admin or UserRole.Seller or UserRole.SupportAgent))throw new UnauthorizedAccessException("Review access is restricted.");}
    public IReadOnlyList<ReviewRecord> Reviews(User u){Staff(u);return repository.Reviews();}
    public IReadOnlyList<ProductQuestionRecord> Questions(User u){Staff(u);return repository.Questions();}
    public void UpdateReview(User u,int id,string status){Staff(u);repository.UpdateReview(id,status);}
    public void Answer(User u,int id,string answer){Staff(u);if(string.IsNullOrWhiteSpace(answer))throw new ArgumentException("Answer is required.");repository.AnswerQuestion(id,answer,"Answered",u.Id);}
}
