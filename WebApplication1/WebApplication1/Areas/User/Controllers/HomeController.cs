using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DataAccess.Repository.IRepository;
using WebApplication1.Models;

namespace WebApplication1.Areas.User.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(ILogger<HomeController> logger,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var collections = await _unitOfWork.Collection.GetAllAsync(includeProperties: "Theme");
            return View(collections);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> LoadDataAsync()
        {
            var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;

            var collections = await _unitOfWork.Collection
                .GetAllAsync(searchText: searchValue, 
                    skipAmount: skip, pageSize: pageSize, 
                    ordering: $"{sortColumn} {sortColumnDirection}",
                    includeProperties: "Theme");

            var recordsTotal = collections.Count();

            return Json(new { Draw = draw, RecordsFiltered = recordsTotal, RecordsTotal = recordsTotal, Data = collections });
        }
    }
}
