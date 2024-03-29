using WebApplication1.DataAccess.Repository.IRepository;
using WebApplication1.Models.ViewModels;
using WebApplication1.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using WebApplication1.Models;
using System.Security.Claims;

namespace WebApplication1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        [BindProperty]
        public UserVM UserVm { get; set; }

        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IUnitOfWork _unitOfWork;
        public UserController(UserManager<IdentityUser> userManager, 
            IUnitOfWork unitOfWork, 
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager) 
        {
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index() 
        {
            var applicationUsers = await _unitOfWork.ApplicationUser.GetAllAsync();
            foreach (var applicationUser in applicationUsers)
            {
                var roles = await _userManager.GetRolesAsync(applicationUser);
                applicationUser.Role = roles.FirstOrDefault()!;
                applicationUser.IsBlocked = applicationUser.LockoutEnd != null;
            }

            UserVm = new UserVM
            {
                ApplicationUsers = applicationUsers
            };

            return View(UserVm);
        }

        public async Task<IActionResult> UserManagement(string userId)
        {
            var applicationUser = await _unitOfWork.ApplicationUser.GetAsync(u => u.Id == userId);

            var roles = await _userManager.GetRolesAsync(applicationUser);
            applicationUser.Role = roles.FirstOrDefault()!;

            applicationUser.IsBlocked = applicationUser.LockoutEnd != null && applicationUser.LockoutEnd > DateTime.Now;

            UserVm = new UserVM
            {
                ApplicationUser = applicationUser,
                RoleList = _roleManager.Roles.Select(u => u.Name).Select(u => new SelectListItem
                {
                    Text = u,
                    Value = u
                })
            };

            return View(UserVm);
        }

        [HttpPost]
        public async Task<IActionResult> UserManagement()
        {
            var applicationUserFromDb = await _unitOfWork.ApplicationUser.GetAsync(u => u.Id == UserVm.ApplicationUser.Id);

            applicationUserFromDb.UserName = UserVm.ApplicationUser.UserName;
            applicationUserFromDb.Name = UserVm.ApplicationUser.Name;
            applicationUserFromDb.Email = UserVm.ApplicationUser.Email;

            _unitOfWork.ApplicationUser.Update(applicationUserFromDb);
            _unitOfWork.Save();

            var oldRole = (await _userManager.GetRolesAsync(applicationUserFromDb)).FirstOrDefault()!;
            if (oldRole != UserVm.ApplicationUser.Role)
            {
                await _userManager.RemoveFromRoleAsync(applicationUserFromDb, oldRole);
                await _userManager.AddToRoleAsync(applicationUserFromDb, UserVm.ApplicationUser.Role);

                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var currentUserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                if (currentUserId == UserVm.ApplicationUser.Id)
                    await _signInManager.SignOutAsync();
            }

            return RedirectToAction(nameof(UserManagement), new { UserId = UserVm.ApplicationUser.Id });
        }

        public async Task<IActionResult> DeleteAsync(string userId)
        {
            var user = await _unitOfWork.ApplicationUser.GetAsync(u => u.Id == userId);

            _unitOfWork.ApplicationUser.Remove(user);
            _unitOfWork.Save();

            TempData["success"] = $"The {user.Name} user has been successfully deleted!";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> LockUnlock(string userId)
        {
            var applicationUser = await _unitOfWork.ApplicationUser.GetAsync(u => u.Id == userId);
            string status;
            if (applicationUser.LockoutEnd != null && applicationUser.LockoutEnd > DateTime.Now)
            {
                applicationUser.LockoutEnd = DateTime.Now;
                status = "unlocked";
            }
            else
            {
                applicationUser.LockoutEnd = DateTime.Now.AddYears(1000);
                status = "blocked";

                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var currentUserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                if (currentUserId == userId)
                    await _signInManager.SignOutAsync();
            }

            _unitOfWork.ApplicationUser.Update(applicationUser);
            _unitOfWork.Save();

            TempData["success"] = $"The {applicationUser.Name} user has been successfully {status}!";

            return RedirectToAction(nameof(UserManagement), new { UserId = applicationUser.Id });
        }
    }
}
