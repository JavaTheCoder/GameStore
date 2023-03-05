using GameStoreData.Identity.Data;
using GameStoreData.Models;
using GameStoreData.Service;
using GameStoreData.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace GameStoreWeb.Controllers
{
    public class GameController : Controller
    {
        private readonly IGameService _service;
        private readonly UserManager<ApplicationUser> _userManager;

        public GameController(IGameService service, UserManager<ApplicationUser> userManager)
        {
            _service = service;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return RedirectToAction("ListGames");
        }

        // ----------------------------------------------------
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

            return RedirectToAction("ListGames");
        }

        public async Task<IActionResult> ElevateUserToManager(string username)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.UserName == username);
            var roles = await _userManager.GetRolesAsync(user);

            await _userManager.RemoveFromRolesAsync(user, roles);
            await _userManager.AddToRoleAsync(user, "Manager");

            return RedirectToAction("ListGames");
        }
        // ----------------------------------------------------

        [HttpGet("Create")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> Create()
        {
            var game = new Game();
            var gameVM = await _service.CreateGameVMAsync(game);

            return View(gameVM);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(GameVM gameVM)
        {
            var game = await _service.CreateGameAsync(gameVM);
            await _service.CreateNewGameAsync(game);

            return RedirectToAction("ListGames");
        }

        [HttpGet("Games")]
        public async Task<IActionResult> ListGames()
        {
            var gameVM = new GameVM();
            gameVM.Games = await _service.GetGamesAsync();

            var genres = await _service.GetGenresAsync();
            _service.InitializeGenresList(gameVM, genres);
            gameVM.SelectedGenreIds = genres.Select(g => g.Id).ToList();

            return View(gameVM);
        }

        [HttpGet("Update/{id}")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> Edit(int id)
        {
            var game = await _service.GetGameByIdAsync(id);
            var gameVM = await _service.CreateGameVMAsync(game);

            return View(gameVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(GameVM gameVM)
        {
            var game = await _service.CreateGameAsync(gameVM);
            await _service.UpdateGameAsync(game);

            return RedirectToAction("ListGames");
        }

        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteGameAsync(id);
            return RedirectToAction("ListGames");
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            TempData.TryGetValue("IsRedirected", out var isRedirected);
            var game = await _service.GetGameAndDeleteInactiveComments((bool?)isRedirected ?? false, id);
            TempData["IsRedirected"] = false;

            return View(game);
        }

        public async Task<IActionResult> FilterGames(GameVM gameVM)
        {
            var gameList = await _service.GetGamesWithSelectedGenresAsync(gameVM.SelectedGenreIds);

            // if filtered by genres - return gameList
            // if filtered by name - return gameList where game.Name.Contains(name)
            gameVM.Games = (gameVM.Name == null) ? gameList : gameList
                    .Where(g => g.Name
                    .Contains(gameVM.Name, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            var genres = await _service.GetGenresAsync();
            _service.InitializeGenresList(gameVM, genres);

            return View("ListGames", gameVM);
        }
    }
}
