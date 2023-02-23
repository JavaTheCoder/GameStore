using GameStoreData.Models;
using GameStoreData.Service;
using GameStoreData.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameStoreWeb.Controllers
{
    public class GameController : Controller
    {
        private readonly GameService _service;

        public GameController(GameService service)
        {
            _service = service;
        }

        public IActionResult Index()
        {
            return RedirectToAction("ListGames");
        }

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
