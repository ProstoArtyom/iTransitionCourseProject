using WebApplication1.DataAccess.Data;
using WebApplication1.DataAccess.Repository.IRepository;
using WebApplication1.Models;

namespace WebApplication1.DataAccess.Repository
{
    public class ItemTagRepository : Repository<ItemTag>, IItemTagRepository
    {
        private readonly ApplicationDbContext _db;
        public ItemTagRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(ItemTag obj)
        {
            _db.ItemTags.Update(obj);
        }
    }
}
