using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.DataAccess.Repository.IRepository;
using WebApplication1.Models;
using WebApplication1.Utility;

namespace WebApplication1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ThemeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ThemeController(IUnitOfWork unitOfWork) 
        { 
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var themes = await _unitOfWork.Theme.GetAllAsync();
            return View(themes);
        }

        public async Task<IActionResult> Upsert(int? themeId)
        {
            Theme theme;
            if (themeId == null)
            {
                theme = new Theme();
            }
            else
            {
                theme = await _unitOfWork.Theme.GetAsync(u => u.Id == themeId);
            }

            return View(theme);
        }

        [HttpPost]
        public async Task<IActionResult> Upsert(Theme theme)
        {
            if (theme.Id == 0)
            {
                _unitOfWork.Theme.AddAsync(theme);
                TempData["success"] = $"The {theme.Name} has been added successfully!";
            }
            else
            {
                _unitOfWork.Theme.Update(theme);
                TempData["success"] = $"The {theme.Name} has been updated successfully!";
            }
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var themes = await _unitOfWork.Theme.GetAllAsync();
            return Json(new { data = themes });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var theme = await _unitOfWork.Theme.GetAsync(u => u.Id == id);
            if (theme == null || theme.Name == "Other")
            {
                return Json(new { success = false, message = "Deleting error!" });
            }

            var themeDefault = await _unitOfWork.Theme.GetAsync(u => u.Name == "Other");
            var collections = await _unitOfWork.Collection.GetAllAsync(u => u.ThemeId == id);
            foreach (var collection in collections)
            {
                collection.ThemeId = themeDefault.Id;
                _unitOfWork.Collection.Update(collection);
            }

            _unitOfWork.Theme.Remove(theme);
            _unitOfWork.Save();

            return Json(new { success = true, message = $"The {theme.Name} theme has been successfully deleted!" });
        }
    }
}
