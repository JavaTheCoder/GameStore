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
            var games = await _service.GetGamesAsync();
            var gameVM = new GameVM
            {
                Games = games
            };

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

        [HttpGet("Game/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var game = await _service.GetGameByIdAsync(id);

            if (game.CommentVM == null)
            {
                game.CommentVM = new CommentVM();
                game.CommentVM.GameId = game.Id;
            }

            game.CommentVM.GameComments = await _service.LoadGameCommentsById(game.Id);

            //var comments = await _service.LoadGameCommentsById(game.Id);

            //foreach (var comment in comments)
            //{
            //    game.CommentVM.GameComments.Add(comment);
            //    // TODO: refactor this
            //    if (comment.Body == game.CommentVM.Body)
            //    {
            //        game.CommentVM.CommentVMId = comment.Id;
            //    }
            //}

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

            if (gameVM.Name == null)
            {
                gameVM.Games = gameList;
            }
            else
            {
                gameVM.Games = new List<Game>();
                foreach (var game in gameList)
                {
                    if (game.Name.Contains(gameVM.Name,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        gameVM.Games.Add(game);
                    }
                }
            }

            var genres = await _service.GetGenresAsync();
            _service.InitializeGenresList(gameVM, genres);

            return View("ListGames", gameVM);
        }
    }
}
