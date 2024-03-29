using WebApplication1.DataAccess.Data;
using WebApplication1.DataAccess.Repository.IRepository;
using WebApplication1.Models;

namespace WebApplication1.DataAccess.Repository
{
    public class LikeRepository : Repository<Like>, ILikeRepository
    {
        private readonly ApplicationDbContext _db;
        public LikeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Like obj)
        {
            _db.Likes.Update(obj);
        }
    }
}
