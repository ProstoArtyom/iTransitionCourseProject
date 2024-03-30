using AspNetCoreWebApp.CloudStorage;
using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Buffers;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
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
        private readonly ICloudStorage _cloudStorage;
        public CollectionController(ILogger<CollectionController> logger,
            IUnitOfWork unitOfWork,
            ICloudStorage cloudStorage)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _cloudStorage = cloudStorage;
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
            var collection = CollectionVm.Collection;
            var collectionFromDb = await _unitOfWork.Collection.GetAsync(u => u.Id == collection.Id);

            if (User.IsInRole(SD.Role_User))
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                if (collectionFromDb.ApplicationUserId != userId)
                {
                    return StatusCode(403);
                }
            }

            collectionFromDb.Name = collection.Name;
            collectionFromDb.Description = collection.Description;
            collectionFromDb.ThemeId = collection.ThemeId;

            if (collection.ImageFile != null)
            {
                collectionFromDb.ImageFile = collection.ImageFile;

                if (collection.ImageStorageName != null)
                {
                    await _cloudStorage.DeleteFileAsync(collection.ImageStorageName);
                }

                await UploadFile(collectionFromDb);
            }

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

        public async Task<IActionResult> ExportToCsv(int collectionId)
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

            var builder = new StringBuilder();
            builder.AppendLine("Id,Name,Description,Theme");
            builder.AppendLine($"{collection.Id},{collection.Name},{collection.Description},{collection.Theme.Name}\n");

            if (collection.Items.Count() > 0)
            {
                builder.AppendLine("Id,Name,Tags");
            }
            foreach (var item in collection.Items)
            {
                var tags = string.Join(", ", item.ItemTags.Select(u => u.Tag.Name));
                builder.AppendLine($"{item.Id},{item.Name},{tags}");
            }

            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "Collection.csv");
        }

        public async Task<IActionResult> ExportCollectionsToCsv()
        {
            Expression<Func<Collection, bool>> filter = u => true;
            if (User.IsAuthenticated() && User.IsInRole(SD.Role_User))
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                filter = u => u.ApplicationUserId == userId;
            }

            var collections = await _unitOfWork.Collection.GetAllAsync(filter, includeProperties: "Theme");
            foreach (var collection in collections)
            {
                var items = await _unitOfWork.Item.GetAllAsync(x => x.CollectionId == collection.Id);
                foreach (var item in items)
                {
                    var itemTags =
                        await _unitOfWork.ItemTag.GetAllAsync(u => u.ItemId == item.Id, includeProperties: "Tag");
                    item.ItemTags = itemTags.ToList();
                }

                collection.Items = items;
            }

            var builder = new StringBuilder();
            foreach (var collection in collections)
            {
                builder.AppendLine("Id,Name,Description,Theme");
                builder.AppendLine($"{collection.Id},{collection.Name},{collection.Description},{collection.Theme.Name}\n");

                if (collection.Items.Count() > 0)
                {
                    builder.AppendLine("Id,Name,Tags");
                }
                foreach (var item in collection.Items)
                {
                    var tags = string.Join(", ", item.ItemTags.Select(u => u.Tag.Name));
                    builder.AppendLine($"{item.Id},{item.Name},{tags}");
                }
                builder.AppendLine();
                builder.AppendLine();
            }

            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "Collections.csv");
        }

        private async Task UploadFile(Collection collection)
        {
            string fileNameForStorage = FormFileName(collection.Name, collection.ImageFile.FileName);
            collection.ImageUrl = await _cloudStorage.UploadFileAsync(collection.ImageFile, fileNameForStorage);
            collection.ImageStorageName = fileNameForStorage;
        }

        private static string FormFileName(string title, string fileName)
        {
            var fileExtension = Path.GetExtension(fileName);
            var fileNameForStorage = $"{title.Replace(' ', '_')}-{DateTime.Now.ToString("yyyyMMddHHmmss")}{fileExtension}";
            return fileNameForStorage;
        }

        [Authorize]
        public async Task<IActionResult> DeleteImageAsync(int collectionId)
        {
            var collection = await _unitOfWork.Collection.GetAsync(u => u.Id == collectionId);

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (collection.ApplicationUserId == userId || User.IsInRole(SD.Role_Admin))
            {
                if (collection.ImageStorageName != null)
                {
                    await _cloudStorage.DeleteFileAsync(collection.ImageStorageName);

                    collection.ImageStorageName = "";
                    collection.ImageUrl = "";

                    _unitOfWork.Collection.Update(collection);
                    _unitOfWork.Save();
                }
            }

            return RedirectToAction(nameof(Index), new { CollectionId = collectionId });
        }
    }
}
