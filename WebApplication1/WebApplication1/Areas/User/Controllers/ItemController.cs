using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using WebApplication1.DataAccess.Repository.IRepository;
using WebApplication1.Models;
using WebApplication1.Models.ViewModels;

namespace WebApplication1.Areas.User.Controllers
{
    public class ItemController : Controller
    {
        [BindProperty]
        public CollectionVM CollectionVm { get; set; }

        private readonly ILogger<ItemController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public ItemController(ILogger<ItemController> logger,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(int itemId)
        {
            var item = await _unitOfWork.Item.GetAsync(u => u.Id == itemId, includeProperties: "Tags");
            return View(item);
        }
    }
}
