using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using WebApplication1.DataAccess.Repository.IRepository;
using WebApplication1.Models;
using WebApplication1.Models.ViewModels;

namespace WebApplication1.Areas.User.Controllers
{
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
        public async Task<IActionResult> Index()
        {
            var collectionFromDb = await _unitOfWork.Collection.GetAsync(u => u.Id == CollectionVm.Collection.Id);

            collectionFromDb.Name = CollectionVm.Collection.Name;
            collectionFromDb.Description = CollectionVm.Collection.Description;
            collectionFromDb.ThemeId = CollectionVm.Collection.ThemeId;

            _unitOfWork.Collection.Update(collectionFromDb);
            _unitOfWork.Save();

            collectionFromDb.Items = await _unitOfWork.Item.GetAllAsync(u => u.CollectionId == collectionFromDb.Id);

            var themesList = await _unitOfWork.Theme.GetAllAsync();

            CollectionVm = new CollectionVM
            {
                Collection = collectionFromDb,
                ThemesList = themesList.Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };

            return View(CollectionVm);
        }
    }
}
