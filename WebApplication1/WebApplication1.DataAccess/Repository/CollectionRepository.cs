using Korzh.EasyQuery.Linq;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebApplication1.DataAccess.Data;
using WebApplication1.DataAccess.Repository.IRepository;
using WebApplication1.Models;

namespace WebApplication1.DataAccess.Repository
{
    public class CollectionRepository : Repository<Collection>, ICollectionRepository
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<Item> _dbSetItems;
        public CollectionRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
            _dbSetItems = _db.Set<Item>();
        }

        public void Update(Collection obj)
        {
            _db.Collections.Update(obj);
        }

        public async Task<IEnumerable<Collection>> GetAllLargestAsync(Expression<Func<Collection, bool>>? filter = null, string includeProperties = null, bool tracked = false, int? count = null)
        {
            IQueryable<Collection> query = dbSet;
            if (tracked) query = dbSet;
            else query = dbSet.AsNoTracking();

            if (filter != null) query = query.Where(filter);

            query = query.OrderByDescending(u => _dbSetItems.Count(i => i.CollectionId == u.Id));

            if (count != null) query = query.Take(count.Value);

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProp in includeProperties
                             .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            return await query.ToListAsync();
        }
    }
}
