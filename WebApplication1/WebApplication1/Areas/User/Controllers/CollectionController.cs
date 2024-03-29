using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using WebApplication1.DataAccess.Repository.IRepository;
using WebApplication1.Models;
using WebApplication1.Models.ViewModels;
using WebApplication1.Utility;

namespace WebApplication1.Areas.User.Controllers
{
    [Area("User")]
    public class CollectionController : Controller
    {
        [BindProperty]
        public CollectionVM CollectionVm { get; set; }

        private readonly ILogger<CollectionController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public CollectionController(ILogger<CollectionController> logger,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(int collectionId)
        {
            var collection = await _unitOfWork.Collection
                .GetAsync(u => u.Id == collectionId, includeProperties: "Theme");

            var items = await _unitOfWork.Item.GetAllAsync(x => x.CollectionId == collectionId);
            foreach (var item in items)
            {
                var itemTags =
                    await _unitOfWork.ItemTag.GetAllAsync(u => u.ItemId == item.Id, includeProperties: "Tag");
                item.ItemTags = itemTags.ToList();
            }

            collection.Items = items;

            var themes = await _unitOfWork.Theme.GetAllAsync();

            CollectionVm = new CollectionVM
            {
                Collection = collection,
                ThemesList = themes.Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };

            return View(CollectionVm);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var collectionFromDb = await _unitOfWork.Collection.GetAsync(u => u.Id == CollectionVm.Collection.Id);

            if (User.IsInRole(SD.Role_User))
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                if (collectionFromDb.ApplicationUserId != userId)
                {
                    return StatusCode(403);
                }
            }

            collectionFromDb.Name = CollectionVm.Collection.Name;
            collectionFromDb.Description = CollectionVm.Collection.Description;
            collectionFromDb.ThemeId = CollectionVm.Collection.ThemeId;

            _unitOfWork.Collection.Update(collectionFromDb);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index), new { CollectionId = collectionFromDb.Id });
        }

        [Authorize]
        public async Task<IActionResult> AddSync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var theme = await _unitOfWork.Theme.GetAsync(u => u.Name == "Other");
            var collection = new Collection
            {
                Name = "",
                Description = "",
                Items = new List<Item>(),
                ThemeId = theme.Id,
                ApplicationUserId = userId
            };
            _unitOfWork.Collection.AddAsync(collection);
            _unitOfWork.Save();

            TempData["success"] = "The collection has been successfully created!";

            return RedirectToAction("List", "Collection");
        }

        [Authorize]
        public async Task<IActionResult> Delete(int collectionId)
        {
            var collection = await _unitOfWork.Collection.GetAsync(u => u.Id == collectionId);

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (collection.ApplicationUserId != userId)
            {
                return StatusCode(403);
            }

            _unitOfWork.Collection.Remove(collection);
            _unitOfWork.Save();

            TempData["success"] = "The collection has been successfully deleted!";

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> List()
        {
            return View();
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

            Expression<Func<Collection, bool>> filter = u => true;
            if (User.IsAuthenticated() && User.IsInRole(SD.Role_User))
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                filter = u => u.ApplicationUserId == userId;
            }

            var collections = await _unitOfWork.Collection
                .GetAllAsync(filter,
                    searchText: searchValue,
                    skipAmount: skip, pageSize: pageSize,
                    ordering: $"{sortColumn} {sortColumnDirection}",
                    includeProperties: "Theme");

            var recordsTotal = collections.Count();

            return Json(new { Draw = draw, RecordsFiltered = recordsTotal, RecordsTotal = recordsTotal, Data = collections });
        }
    }
}
