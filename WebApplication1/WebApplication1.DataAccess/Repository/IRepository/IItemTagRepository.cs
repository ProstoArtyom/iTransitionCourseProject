using WebApplication1.Models;

namespace WebApplication1.DataAccess.Repository.IRepository
{
    public interface IItemTagRepository : IRepository<ItemTag>
    {
        void Update(ItemTag obj);
    }
}
