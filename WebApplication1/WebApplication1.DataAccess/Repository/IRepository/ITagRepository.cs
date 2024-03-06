using WebApplication1.Models;

namespace WebApplication1.DataAccess.Repository.IRepository
{
    public interface ITagRepository : IRepository<Tag>
    {
        void Update(Tag obj);
    }
}
