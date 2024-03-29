using System.Linq.Expressions;

namespace WebApplication1.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null, bool tracked = false);
        T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false);
        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);

        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, 
            string? searchText = null, 
            int? skipAmount = null, int? pageSize = null,
            string? ordering = null,
            string? includeProperties = null, 
            bool tracked = false);
        Task<T> GetAsync(Expression<Func<T, bool>> filter, string? searchText = null, int? skipAmount = null, int? pageSize = null, string? includeProperties = null, bool tracked = false);
        void AddAsync(T entity);
        Task<int> GetCountAsync(Expression<Func<T, bool>>? filter);
    }
}
