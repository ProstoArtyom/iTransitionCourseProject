using WebApplication1.Models;

namespace WebApplication1.DataAccess.Repository.IRepository
{
    public interface IThemeRepository : IRepository<Theme>
    {
        void Update(Theme obj);
    }
}
