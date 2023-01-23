using GameStoreData.Identity.Data;
using GameStoreData.Models;
using GameStoreData.Service;
using GameStoreData.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameStoreWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly GameService _service;

        public HomeController(GameService service)
        {
            _service = service;
        }

        //public async Task<IActionResult> Index()
        public IActionResult Index()
        {
            //var genre1 = new Genre { Name = "RPG" };
            //var genre2 = new Genre { Name = "Adventure" };
            //var game1 = new Game { Name = "Horizon: Zero Dawn", Image = "horizon-zero-dawn", Price = 54.99 };
            //var game2 = new Game { Name = "Horizon: Forbidden West", Image = "horizon-forbidden-west", Price = 59.99 };

            //genre1.Games.Add(game1);
            //genre1.Games.Add(game2);

            //genre2.Games.Add(game1);
            //genre2.Games.Add(game2);

            //game1.Genres.Add(genre1);
            //game2.Genres.Add(genre1);

            //game1.Genres.Add(genre2);
            //game2.Genres.Add(genre2);

            //await _context.Genres.AddAsync(genre1);
            //await _context.Genres.AddAsync(genre2);
            //await _context.Games.AddAsync(game1);
            //await _context.Games.AddAsync(game2);

            //await _context.SaveChangesAsync();

            //var genre1 = new Genre { Name = "Strategy" };
            //var genre2 = new Genre { Name = "FPS" };
            //var genre3 = new Genre { Name = "Arcade" };
            //var genre4 = new Genre { Name = "Sports" };
            //var genre5 = new Genre { Name = "Action" };

            //await _context.Genres.AddAsync(genre1);
            //await _context.Genres.AddAsync(genre2);
            //await _context.Genres.AddAsync(genre3);
            //await _context.Genres.AddAsync(genre4);
            //await _context.Genres.AddAsync(genre5);

            //await _context.SaveChangesAsync();
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
            var games = await _service.GetGamesAsync();
            return View(games);
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
            await _service.UpdateGameAsync(game); //TODO: remove existing genres

            return RedirectToAction("ListGames");
        }

        [HttpGet("Game/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var game = await _service.GetGameByIdAsync(id);
            return View(game);
        }

        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteGameAsync(id);
            return RedirectToAction("ListGames");
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}