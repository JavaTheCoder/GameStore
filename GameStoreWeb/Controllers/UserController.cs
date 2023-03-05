using GameStoreData.Identity.Data;
using GameStoreData.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GameStoreWeb.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult ElevateUsers()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        public async Task<IActionResult> ElevateUserToAdmin(string username)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.UserName == username);
            var roles = await _userManager.GetRolesAsync(user);

            await _userManager.RemoveFromRolesAsync(user, roles);
            await _userManager.AddToRoleAsync(user, "Admin");

            return RedirectToAction("ListGames", "Game");
        }

        public async Task<IActionResult> ElevateUserToManager(string username)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.UserName == username);
            var roles = await _userManager.GetRolesAsync(user);

            await _userManager.RemoveFromRolesAsync(user, roles);
            await _userManager.AddToRoleAsync(user, "Manager");

            return RedirectToAction("ListGames", "Game");
        }
    }
}
