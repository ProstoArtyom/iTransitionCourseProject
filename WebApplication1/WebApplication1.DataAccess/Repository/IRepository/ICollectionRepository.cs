using WebApplication1.Models;

namespace WebApplication1.DataAccess.Repository.IRepository
{
    public interface ICollectionRepository : IRepository<Collection>
    {
        void Update(Collection obj);
    }
}
