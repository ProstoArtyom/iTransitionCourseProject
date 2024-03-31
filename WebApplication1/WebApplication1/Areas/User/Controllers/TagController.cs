using Microsoft.AspNetCore.Mvc;
using WebApplication1.DataAccess.Repository.IRepository;

namespace WebApplication1.Areas.User.Controllers
{
    [Area("User")]
    public class TagController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public TagController(IUnitOfWork unitOfWork) 
        { 
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<JsonResult> GetAllTags(string prefix)
        {
            var tags = await _unitOfWork.Tag.GetAllAsync(u => u.Name.StartsWith(prefix));
            return Json(tags);
        }
    }
}
