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
        public async Task<IActionResult> Details(int id, bool redirected)
        {
            var game = await _service.GetGameByIdAsync(id);
            if (redirected == false)
            {
                var comments = await _service.LoadGameCommentsById(game.Id);
                var inactiveComments = comments.Where(c => c.IsActive == false);
                foreach (var c in inactiveComments)
                {
                    await _service.DeleteCommentAsync(c);
                }
            }

            game.CommentVM = new CommentVM
            {
                GameId = game.Id,
                GameComments = await
                    _service.LoadGameCommentsById(game.Id)
            };

            return View(game);
        }

        public async Task<IActionResult> FilterGames(GameVM gameVM)
        {
            foreach (int id in gameVM.SelectedGenreIds)
            {
                gameVM.Genres.Add(await _service.GetGenreById(id));
            }

            var gameList = new List<Game>();
            foreach (var genre in gameVM.Genres)
            {
                foreach (var game in genre.Games)
                {
                    if (!gameList.Contains(game))
                    {
                        gameList.Add(await _service.GetGameByIdAsync(game.Id));
                    }
                }
            }

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
