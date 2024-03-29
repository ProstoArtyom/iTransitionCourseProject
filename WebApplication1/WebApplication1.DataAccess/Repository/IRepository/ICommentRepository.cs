using WebApplication1.Models;

namespace WebApplication1.DataAccess.Repository.IRepository
{
    public interface ICommentRepository : IRepository<Comment>
    {
        void Update(Comment obj);
    }
}
