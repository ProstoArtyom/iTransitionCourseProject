using WebApplication1.Models;

namespace WebApplication1.DataAccess.Repository.IRepository
{
    public interface IItemRepository : IRepository<Item>
    {
        void Update(Item obj);
    }
}
