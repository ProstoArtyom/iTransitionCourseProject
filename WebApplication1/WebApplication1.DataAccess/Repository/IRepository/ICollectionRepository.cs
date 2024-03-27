using System.Linq.Expressions;
using WebApplication1.Models;

namespace WebApplication1.DataAccess.Repository.IRepository
{
    public interface ICollectionRepository : IRepository<Collection>
    {
        void Update(Collection obj);
        Task<IEnumerable<Collection>> GetAllLargestAsync(Expression<Func<Collection, bool>>? filter = null,
            string? includeProperties = null, bool tracked = false, int? count = null);
    }
}
