using Microsoft.AspNetCore.Mvc;
using WebApplication1.DataAccess.Repository.IRepository;
using WebApplication1.Models;
using WebApplication1.Models.ViewModels;

namespace WebApplication1.Areas.User.Controllers
{
    [Area("User")]
    public class SearchController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public SearchController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index(string searchText)
        {
            var items = (await _unitOfWork.Item.GetAllAsync(searchText: searchText)).ToList();
            foreach (var item in items)
            {
                var itemTagsForItem = await _unitOfWork.ItemTag.GetAllAsync(i => i.ItemId == item.Id, includeProperties: "Tag");
                item.ItemTags = itemTagsForItem.ToList();
            }
            var itemTags = await _unitOfWork.ItemTag.GetAllAsync(u => u.Tag.Name.Contains(searchText), includeProperties: "Tag");
            foreach (var itemTag in itemTags)
            {
                if (!items.Any(u => u.Id == itemTag.ItemId))
                {
                    var item = await _unitOfWork.Item.GetAsync(u => u.Id == itemTag.ItemId);
                    item.ItemTags = (await _unitOfWork.ItemTag.GetAllAsync(u => u.ItemId == item.Id, includeProperties: "Tag")).ToList();
                    items.Add(item);
                }
            }

            var collections = (await _unitOfWork.Collection.GetAllAsync(searchText: searchText, includeProperties: "Theme")).ToList();

            var searchVm = new SearchVM
            {
                Items = items,
                Collections = collections,
                SearchText = searchText,
            };

            return View(searchVm);
        }

        public async Task<IActionResult> SearchByTag(int tagId)
        {
            var itemTags = await _unitOfWork.ItemTag.GetAllAsync(u => u.TagId == tagId, includeProperties: "Item");
            var items = itemTags.Select(u => u.Item);
            foreach (var item in items)
            {
                var itemTagsForItem = await _unitOfWork.ItemTag.GetAllAsync(u => u.ItemId == item.Id, includeProperties: "Tag");
                item.ItemTags = itemTagsForItem.ToList();
            }

            var tag = await _unitOfWork.Tag.GetAsync(u => u.Id == tagId);
            var searchVm = new SearchVM
            {
                Items = items,
                Collections = new List<Collection>(),
                SearchText = tag.Name
            };

            return View("Index", searchVm);
        }
    }
}
