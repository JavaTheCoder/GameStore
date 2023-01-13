using GameStoreData;
using GameStoreWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            //var genre1 = new Genre { Name = "RPG" };
            //var game1 = new Game { Name = "Horizon: Zero Dawn", Price = 54.99 };
            //var game2 = new Game { Name = "Horizon: Forbidden West", Price = 59.99 };

            //genre1.Games.Add(game1);
            //genre1.Games.Add(game2);
            //game1.Genres.Add(genre1);
            //game2.Genres.Add(genre1);

            //await _context.Genres.AddAsync(genre1);
            //await _context.Games.AddAsync(game1);
            //await _context.Games.AddAsync(game2);

            //await _context.SaveChangesAsync();

            //var game1 = await _context.Games.FirstOrDefaultAsync(g => g.Name.Contains("Zero"));
            return View();
        }

        [HttpGet("Games")]
        public async Task<IActionResult> ListGames()
        {
            var games = await _context.Games.ToListAsync();
            return View(games);
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