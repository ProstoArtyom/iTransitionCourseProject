using WebApplication1.DataAccess.Data;
using WebApplication1.DataAccess.Repository.IRepository;
using WebApplication1.Models;

namespace WebApplication1.DataAccess.Repository
{
    public class CollectionRepository : Repository<Collection>, ICollectionRepository
    {
        private readonly ApplicationDbContext _db;
        public CollectionRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Collection obj)
        {
            _db.Collections.Update(obj);
        }
    }
}
