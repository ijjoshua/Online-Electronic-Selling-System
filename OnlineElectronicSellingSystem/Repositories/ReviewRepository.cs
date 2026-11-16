using MySqlConnector;
using OnlineElectronicSellingSystem.Data;
using OnlineElectronicSellingSystem.Models;

namespace OnlineElectronicSellingSystem.Repositories;

public sealed class ReviewRepository
{
    private readonly Database database;
    public ReviewRepository(Database database) => this.database = database;

    public IReadOnlyList<ReviewRecord> Reviews()
    {
        using var c=database.OpenConnection(); using var cmd=new MySqlCommand(@"SELECT r.id,p.name,u.full_name,r.rating,r.comment,r.status,r.is_verified_buyer,r.created_at FROM reviews r JOIN products p ON p.id=r.product_id JOIN users u ON u.id=r.user_id ORDER BY r.created_at DESC",c); using var rd=cmd.ExecuteReader(); var a=new List<ReviewRecord>(); while(rd.Read()) a.Add(new ReviewRecord{Id=rd.GetInt32("id"),ProductName=rd.GetString("name"),CustomerName=rd.GetString("full_name"),Rating=rd.GetByte("rating"),Comment=rd.IsDBNull(rd.GetOrdinal("comment"))?null:rd.GetString("comment"),Status=rd.GetString("status"),VerifiedBuyer=rd.GetBoolean("is_verified_buyer"),CreatedAt=rd.GetDateTime("created_at")}); return a;
    }
    public IReadOnlyList<ProductQuestionRecord> Questions()
    {
        using var c=database.OpenConnection(); using var cmd=new MySqlCommand(@"SELECT q.id,p.name,u.full_name,q.question,q.answer,q.status,q.created_at FROM product_questions q JOIN products p ON p.id=q.product_id JOIN users u ON u.id=q.user_id ORDER BY q.created_at DESC",c); using var rd=cmd.ExecuteReader(); var a=new List<ProductQuestionRecord>(); while(rd.Read()) a.Add(new ProductQuestionRecord{Id=rd.GetInt32("id"),ProductName=rd.GetString("name"),CustomerName=rd.GetString("full_name"),Question=rd.GetString("question"),Answer=rd.IsDBNull(rd.GetOrdinal("answer"))?null:rd.GetString("answer"),Status=rd.GetString("status"),CreatedAt=rd.GetDateTime("created_at")}); return a;
    }
    public void UpdateReview(int id,string status){using var c=database.OpenConnection();using var cmd=new MySqlCommand("UPDATE reviews SET status=@status WHERE id=@id",c);cmd.Parameters.AddWithValue("@status",status);cmd.Parameters.AddWithValue("@id",id);cmd.ExecuteNonQuery();}
    public void AnswerQuestion(int id,string answer,string status,int userId){using var c=database.OpenConnection();using var cmd=new MySqlCommand("UPDATE product_questions SET answer=@answer,answered_by=@user,status=@status,answered_at=CURRENT_TIMESTAMP WHERE id=@id",c);cmd.Parameters.AddWithValue("@answer",answer);cmd.Parameters.AddWithValue("@user",userId);cmd.Parameters.AddWithValue("@status",status);cmd.Parameters.AddWithValue("@id",id);cmd.ExecuteNonQuery();}
}
