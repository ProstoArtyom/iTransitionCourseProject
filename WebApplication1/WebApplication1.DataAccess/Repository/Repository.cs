using System.Linq.Expressions;
using Korzh.EasyQuery.Linq;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using WebApplication1.DataAccess.Data;
using WebApplication1.DataAccess.Repository.IRepository;

namespace WebApplication1.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;
        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }

        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false)
        {
            IQueryable<T> query;
            if (tracked) query = dbSet;
            else query = dbSet.AsNoTracking();

            query = query.Where(filter);
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties
                             .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query.FirstOrDefault();
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter, string? includeProperties = null, bool tracked = false)
        {
            IQueryable<T> query = dbSet;
            if (tracked) query = dbSet;
            else query = dbSet.AsNoTracking();

            if (filter != null) query = query.Where(filter);

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties
                             .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query.ToList();
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
        }

        public async void AddAsync(T entity)
        {
            await dbSet.AddAsync(entity);
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> filter, string? searchText = null, int? skipAmount = null, int? pageSize = null, string? includeProperties = null, bool tracked = false)
        {
            IQueryable<T> query;
            if (tracked) query = dbSet;
            else query = dbSet.AsNoTracking();

            query = query.Where(filter);

            if (skipAmount != null) query = query.Skip(skipAmount.Value);
            if (pageSize != null) query = query.Take(pageSize.Value);

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties
                             .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter, 
            string? searchText = null, 
            int? skipAmount = null, int? pageSize = null, 
            string? ordering = null, 
            string? includeProperties = null, 
            bool tracked = false)
        {
            IQueryable<T> query = dbSet;
            if (tracked) query = dbSet;
            else query = dbSet.AsNoTracking();

            if (filter != null) query = query.Where(filter);

            if (searchText != null) query = query.FullTextSearchQuery(searchText);

            if (skipAmount != null) query = query.Skip(skipAmount.Value);
            if (pageSize != null) query = query.Take(pageSize.Value);

            if (!string.IsNullOrWhiteSpace(ordering)) query = query.OrderBy(ordering);

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

        public async Task<int> GetCountAsync(Expression<Func<T, bool>>? filter)
        {
            IQueryable<T> query = dbSet.AsNoTracking();

            if (filter != null)
                query = query.Where(filter);

            return await query.CountAsync();
        }
    }
}
