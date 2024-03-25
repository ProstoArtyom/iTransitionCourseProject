using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using WebApplication1.DataAccess.Repository.IRepository;
using WebApplication1.Models;
using WebApplication1.Models.ViewModels;
using Newtonsoft.Json;

namespace WebApplication1.Areas.User.Controllers
{
    public class ItemController : Controller
    {
        [BindProperty]
        public ItemVM ItemVm { get; set; }

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
            var item = await _unitOfWork.Item.GetAsync(u => u.Id == itemId);
            var itemTags =
                await _unitOfWork.ItemTag.GetAllAsync(u => u.ItemId == item.Id, includeProperties: "Tag");
            item.ItemTags = itemTags.ToList();
            var customFields = JsonConvert.DeserializeObject<Dictionary<string, object[]>>(item.CustomFields);

            ItemVm = new()
            {
                Item = item,
                CustomFields = customFields
            };

            return View(ItemVm);
        }

        public async Task<IActionResult> AddTagAsync(ItemVM obj)
        {
            var tagName = obj.TagName?.Trim();
            if (tagName == null)
            {
                TempData["error"] = "The TagName field is required.";
            }
            else if (tagName.Length < 5)
            {
                TempData["error"] = "The TagName field's length can't be less than 5.";
            }
            else if (tagName.Length > 20)
            {
                TempData["error"] = "The TagName field's length can't be greater than 20.";
            }
            else if (Regex.IsMatch(tagName, "/#\\w+/gm"))
            {
                TempData["error"] = "Incorrect characters for the TagName field.";
            }
            else
            {
                var tag = await _unitOfWork.Tag.GetAsync(u => u.Name == tagName);
                var item = await _unitOfWork.Item.GetAsync(u => u.Id == obj.Item.Id, includeProperties: "ItemTags");
                if (tag == null)
                {
                    tag = new Tag
                    {
                        Name = tagName,
                    };
                    _unitOfWork.Tag.AddAsync(tag);
                    _unitOfWork.Save();
                }
                else if (item.ItemTags.Exists(u => u.TagId == tag.Id))
                {
                    TempData["error"] = $"The {tag.Name} tag already exists.";
                    return RedirectToAction(nameof(Index), new { ItemId = obj.Item.Id });
                }

                TempData["success"] = $"The {tag.Name} tag has been added!";

                var itemTag = new ItemTag
                {
                    ItemId = item.Id,
                    TagId = tag.Id
                };
                _unitOfWork.ItemTag.AddAsync(itemTag);
                _unitOfWork.Save();
            }

            return RedirectToAction(nameof(Index), new { ItemId = obj.Item.Id });
        }

        public async Task<IActionResult> RemoveTagAsync(int tagId, int itemId)
        {
            var itemTag = await _unitOfWork.ItemTag.GetAsync(u => u.ItemId == itemId && u.TagId == tagId);
            _unitOfWork.ItemTag.Remove(itemTag);
            _unitOfWork.Save();

            var tag = await _unitOfWork.Tag.GetAsync(u => u.Id == itemTag.TagId);
            TempData["success"] = $"The {tag.Name} tag has been removed!";

            return RedirectToAction(nameof(Index), new { ItemId = itemId });
        }

        public async Task<IActionResult> AddFieldAsync(int itemId, string fieldName, string fieldValue, string fieldType)
        {


            var item = await _unitOfWork.Item.GetAsync(u => u.Id == itemId);
            
            var customFields = JsonConvert.DeserializeObject<Dictionary<string, object[]>>(item.CustomFields)!;
            customFields.Add(fieldName, new object[] { fieldValue, fieldType });
            var customFieldsJson = JsonConvert.SerializeObject(customFields);

            item.CustomFields = customFieldsJson;
            _unitOfWork.Item.Update(item);
            _unitOfWork.Save();

            TempData["success"] = $"The {fieldName} field has been successfully added!";

            return RedirectToAction(nameof(Index), new { ItemId = itemId });
        }
    }
}
