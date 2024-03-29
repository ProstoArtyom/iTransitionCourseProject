using WebApplication1.Models;

namespace WebApplication1.DataAccess.Repository.IRepository
{
    public interface ILikeRepository : IRepository<Like>
    {
        void Update(Like obj);
    }
}
