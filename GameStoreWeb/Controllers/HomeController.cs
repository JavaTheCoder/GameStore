using GameStoreData;
using GameStoreData.Identity.Data;
using GameStoreData.Models;
using GameStoreData.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Cryptography.Xml;

namespace GameStoreWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly GameService _service;

        public HomeController(ApplicationDbContext context, GameService service)
        {
            _context = context;
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
        public async Task<IActionResult> Create()
        {
            var game = new Game();
            ViewBag.Genres = await _service.GetGenresAsync();
            return View(game);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(Game game)
        {
            await _service.CreateGameAsync(game);
            return RedirectToAction("ListGames");
        }

        [HttpGet("Games")]
        public async Task<IActionResult> ListGames()
        {
            var games = await _service.GetGamesAsync();
            return View(games);
        }

        [HttpGet("Update/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var game = await _service.GetGameByIdAsync(id);
            var genres = await _service.GetGenresAsync();

            //var list = new List<SelectListItem>();
            //foreach (var genre in genres)
            //{
            //    list.Add(new SelectListItem()
            //    {
            //        Text = genre.Name,
            //        Value = genre.Name
            //    });
            //}

            //game.SelectedGenres = list;
            ViewBag.Genres = genres;
            return View(game);
        }

        public async Task<IActionResult> Update(Game game)
        {
            await _service.UpdateGameAsync(game);
            return RedirectToAction("ListGames");
        }

        [HttpGet("Game/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var game = await _service.GetGameByIdAsync(id);
            return View(game);
        }

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