using GameStoreData;
using GameStoreData.Models;
using GameStoreWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GameStoreWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;


        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var game1 = new Game { Name = "The Witcher 3: Wild Hunt", Genres = new List<Genre>(), Price = 49.99 };
            var game2 = new Game { Name = "Battlefield 4", Genres = new List<Genre>(), Price = 39.99 };
            var genre1 = new Genre { Name = "Adventure", Games = new List<Game>() };

            genre1.Games.Add(game1);
            genre1.Games.Add(game2);

            game1.Genres.Add(genre1);
            game2.Genres.Add(genre1);

            await _context.Genres.AddAsync(genre1);
            await _context.Games.AddAsync(game1);
            await _context.Games.AddAsync(game2);

            await _context.SaveChangesAsync();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}