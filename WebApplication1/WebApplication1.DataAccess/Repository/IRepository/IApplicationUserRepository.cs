using WebApplication1.Models;

namespace WebApplication1.DataAccess.Repository.IRepository;

public interface IApplicationUserRepository : IRepository<ApplicationUser>
{
    void Update(ApplicationUser obj);
}
