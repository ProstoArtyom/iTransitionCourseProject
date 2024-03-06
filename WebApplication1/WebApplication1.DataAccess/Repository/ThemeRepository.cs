using WebApplication1.DataAccess.Data;
using WebApplication1.DataAccess.Repository.IRepository;
using WebApplication1.Models;

namespace WebApplication1.DataAccess.Repository
{
    public class ThemeRepository : Repository<Theme>, IThemeRepository
    {
        private readonly ApplicationDbContext _db;
        public ThemeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Theme obj)
        {
            _db.Themes.Update(obj);
        }
    }
}
