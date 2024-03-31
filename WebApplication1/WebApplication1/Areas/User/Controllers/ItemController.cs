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
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using WebApplication1.Utility;
using Duende.IdentityServer.Extensions;

namespace WebApplication1.Areas.User.Controllers
{
    [Area("User")]
    public class ItemController : Controller
    {
        [BindProperty]
        public ItemVM ItemVm { get; set; } = new();

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
            var collection = await _unitOfWork.Collection.GetAsync(u => u.Id == item.CollectionId);
            var itemTags =
                await _unitOfWork.ItemTag.GetAllAsync(u => u.ItemId == itemId, includeProperties: "Tag");
            item.ItemTags = itemTags.ToList();

            Dictionary<string, string[]> customFields;
            if (item.CustomFields != null)
                customFields = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(item.CustomFields);
            else
                customFields = new Dictionary<string, string[]>();

            var comments = await _unitOfWork.Comment.GetAllAsync(u => u.ItemId == itemId, includeProperties: "ApplicationUser");

            bool isLiked = false;
            if (User.IsAuthenticated())
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                var like = await _unitOfWork.Like.GetAsync(u => u.ItemId == itemId && u.ApplicationUserId == userId);
                isLiked = like != null;
            }

            var likesCount = await _unitOfWork.Like.GetCountAsync(u => u.ItemId == itemId);

            ItemVm = new()
            {
                Item = item,
                CustomFields = customFields,
                UserId = collection.ApplicationUserId,
                Comments = comments,
                IsLiked = isLiked,
                LikesCount = likesCount
            };

            return View(ItemVm);
        }

        [Authorize]
        public async Task<IActionResult> Create(int collectionId)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var collection = await _unitOfWork.Collection.GetAsync(u => u.Id == collectionId);

            if (User.IsInRole(SD.Role_User) && collection.ApplicationUserId != userId)
            {
                return StatusCode(403);
            }

            ItemVm = new()
            {
                Item = new Item
                {
                    CollectionId = collectionId,
                    CustomFields = JsonConvert.SerializeObject(new Dictionary<string, string[]>())
                },
                UserId = collection.ApplicationUserId
            };

            return View(ItemVm);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var item = ItemVm.Item;

            _unitOfWork.Item.AddAsync(item);
            _unitOfWork.Save();

            TempData["success"] = "The item has been successfully added!";

            return RedirectToAction("Index", "Collection", new { CollectionId = item.CollectionId });
        }

        [Authorize]
        public async Task<IActionResult> Edit(int itemId)
        {
            var item = await _unitOfWork.Item.GetAsync(u => u.Id == itemId);
            var collection = await _unitOfWork.Collection.GetAsync(u => u.Id == item.CollectionId);

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (User.IsInRole(SD.Role_User) && collection.ApplicationUserId != userId)
            {
                return StatusCode(403);
            }

            var itemTags =
                await _unitOfWork.ItemTag.GetAllAsync(u => u.ItemId == itemId, includeProperties: "Tag");
            item.ItemTags = itemTags.ToList();

            Dictionary<string, string[]> customFields;
            if (item.CustomFields != null)
                customFields = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(item.CustomFields);
            else
                customFields = new Dictionary<string, string[]>();

            ItemVm = new()
            {
                Item = item,
                CustomFields = customFields,
                UserId = collection.ApplicationUserId
            };

            return View(ItemVm);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit()
        {
            var item = await _unitOfWork.Item.GetAsync(u => u.Id == ItemVm.Item.Id);
            var collection = await _unitOfWork.Collection.GetAsync(u => u.Id == item.CollectionId);

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (User.IsInRole(SD.Role_User) && collection.ApplicationUserId != userId)
            {
                return StatusCode(403);
            }

            if (ItemVm.CustomFields != null)
            {
                foreach (var field in ItemVm.CustomFields)
                {
                    string inputValue, textAreaValue, type;
                    if (field.Value.Length > 0)
                    {
                        inputValue = field.Value[0];
                        textAreaValue = field.Value[1];
                        type = field.Value[2];

                        ItemVm.CustomFields[field.Key] = new[]
                        {
                            type == "textarea"
                                ? textAreaValue
                                : inputValue,
                            type
                        };
                    }
                    else
                    {
                        inputValue = "false";
                        type = "checkbox";

                        ItemVm.CustomFields[field.Key] = new[] { inputValue, type };
                    }
                }

                item.CustomFields = JsonConvert.SerializeObject(ItemVm.CustomFields);
            }

            item.Name = ItemVm.Item.Name;

            _unitOfWork.Item.Update(item);
            _unitOfWork.Save();

            TempData["success"] = "The item has been successfully updated";

            return RedirectToAction(nameof(Edit), new { ItemId = ItemVm.Item.Id });
        }

        [Authorize]
        public async Task<IActionResult> Delete(int itemId)
        {
            var item = await _unitOfWork.Item.GetAsync(u => u.Id == itemId);
            var collection = await _unitOfWork.Collection.GetAsync(u => u.Id == item.CollectionId);

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (User.IsInRole(SD.Role_User) && collection.ApplicationUserId != userId)
            {
                return StatusCode(403);
            }

            _unitOfWork.Item.Remove(item);
            _unitOfWork.Save();

            return RedirectToAction("Index", "Collection",new { collectionId = item.CollectionId });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddTagAsync(ItemVM obj)
        {
            var item = await _unitOfWork.Item.GetAsync(u => u.Id == obj.Item.Id, includeProperties: "ItemTags");
            var collection = await _unitOfWork.Collection.GetAsync(u => u.Id == item.CollectionId);

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (User.IsInRole(SD.Role_User) && collection.ApplicationUserId != userId)
            {
                return StatusCode(403);
            }

            var tagName = obj.TagName?.Trim();
            if (tagName != null)
            {
                var tag = await _unitOfWork.Tag.GetAsync(u => u.Name == tagName);
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
                    return RedirectToAction(nameof(Edit), new { ItemId = obj.Item.Id });
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

            return RedirectToAction(nameof(Edit), new { ItemId = obj.Item.Id });
        }

        [Authorize]
        public async Task<IActionResult> RemoveTagAsync(int tagId, int itemId)
        {
            var item = await _unitOfWork.Item.GetAsync(u => u.Id == itemId);
            var collection = await _unitOfWork.Collection.GetAsync(u => u.Id == item.CollectionId);

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (User.IsInRole(SD.Role_User) && collection.ApplicationUserId != userId)
            {
                return StatusCode(403);
            }

            var itemTag = await _unitOfWork.ItemTag.GetAsync(u => u.ItemId == itemId && u.TagId == tagId);
            _unitOfWork.ItemTag.Remove(itemTag);
            _unitOfWork.Save();

            var tag = await _unitOfWork.Tag.GetAsync(u => u.Id == itemTag.TagId);
            TempData["success"] = $"The {tag.Name} tag has been removed!";

            return RedirectToAction(nameof(Edit), new { ItemId = itemId });
        }

        [Authorize]
        public async Task<IActionResult> AddFieldAsync(ItemVM obj)
        {
            var item = await _unitOfWork.Item.GetAsync(u => u.Id == obj.Item.Id);
            var collection = await _unitOfWork.Collection.GetAsync(u => u.Id == item.CollectionId);

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (User.IsInRole(SD.Role_User) && collection.ApplicationUserId != userId)
            {
                return StatusCode(403);
            }

            var customFields = JsonConvert.DeserializeObject<Dictionary<string, object[]>>(item.CustomFields)!;
            if (customFields.ContainsKey(obj.FieldName))
            {
                TempData["error"] = $"Field with {obj.FieldName} name already exists!";
            }
            else
            {
                customFields.Add(obj.FieldName, new object[]
                {
                    obj.FieldType == "textarea" ? obj.FieldValueTextArea : obj.FieldValueInput,
                    obj.FieldType
                });
                var customFieldsJson = JsonConvert.SerializeObject(customFields);

                item.CustomFields = customFieldsJson;
                _unitOfWork.Item.Update(item);
                _unitOfWork.Save();

                TempData["success"] = $"The {obj.FieldName} field has been successfully added!";
            }

            return RedirectToAction(nameof(Edit), new { ItemId = obj.Item.Id });
        }

        [Authorize]
        public async Task<IActionResult> RemoveFieldAsync(string fieldKey, int itemId)
        {
            var item = await _unitOfWork.Item.GetAsync(u => u.Id == itemId);
            var collection = await _unitOfWork.Collection.GetAsync(u => u.Id == item.CollectionId);

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (User.IsInRole(SD.Role_User) && collection.ApplicationUserId != userId)
            {
                return StatusCode(403);
            }

            var customFields = JsonConvert.DeserializeObject<Dictionary<string, object[]>>(item.CustomFields)!;
            customFields.Remove(fieldKey);
            var customFieldsJson = JsonConvert.SerializeObject(customFields);

            item.CustomFields = customFieldsJson;

            _unitOfWork.Item.Update(item);
            _unitOfWork.Save();

            TempData["success"] = $"The {fieldKey} field has been removed!";

            return RedirectToAction(nameof(Edit), new { ItemId = itemId });
        }

        [Authorize]
        public IActionResult AddComment()
        {
            var comment = new Comment
            {
                Text = ItemVm.CommentText,
                ApplicationUserId = ItemVm.UserId,
                ItemId = ItemVm.Item.Id
            };

            _unitOfWork.Comment.AddAsync(comment);
            _unitOfWork.Save();

            TempData["success"] = "The comment has been added successfully!";

            return RedirectToAction("Index", "Item", new { ItemId = ItemVm.Item.Id });
        }

        [Authorize]
        public IActionResult AddLike()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var like = new Like
            {
                ItemId = ItemVm.Item.Id,
                ApplicationUserId = userId
            };

            _unitOfWork.Like.AddAsync(like);
            _unitOfWork.Save();

            return RedirectToAction("Index", "Item", new { ItemId = ItemVm.Item.Id });
        }

        [Authorize]
        public async Task<IActionResult> RemoveLike()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var like = await _unitOfWork.Like.GetAsync(u => u.ItemId == ItemVm.Item.Id
                                                        && u.ApplicationUserId == userId);
            _unitOfWork.Like.Remove(like);
            _unitOfWork.Save();

            return RedirectToAction("Index", "Item", new { ItemId = ItemVm.Item.Id });
        }
    }
}
