using GameStoreData.Identity.Data;
using GameStoreData.Models;
using GameStoreData.Service;
using GameStoreData.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GameStoreWeb.Controllers
{
    public class GameController : Controller
    {
        private readonly GameService _service;
        private readonly UserManager<ApplicationUser> _userManager;

        public GameController(GameService service, UserManager<ApplicationUser> userManager)
        {
            _service = service;
            _userManager = userManager;
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

            foreach (var comment in await _service.LoadGameCommentsById(game.Id))
            {
                game.CommentVM.GameComments.Add(comment);
                if (comment.Body == game.CommentVM.Body)
                {
                    game.CommentVM.CommentVMId = comment.Id;
                }
            }

            return View(game);
        }

        // ------------------------- Filtering (Epic 1) -------------------------
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

        // -------------------------Handling Carts (Epic 4)-------------------------
        [HttpGet]
        public async Task<IActionResult> AddToCart(int id)
        {
            string username = _userManager.GetUserAsync(User).Result.UserName;
            var game = await _service.GetGameByIdAsync(id);

            var cart = _service.GetAllCartItemsByUsername(username)
                .FirstOrDefault(c => c?.GameInCart?.Name == game.Name);
            if (cart != null)
            {
                cart.Quantity += 1;
                await _service.UpdateCartAsync(cart);
                return RedirectToAction("ListGames");
            }

            await _service.SaveCartAsync(
                new CartItem
                {
                    Quantity = 1,
                    GameInCart = game,
                    Username = username
                }
            );
            return RedirectToAction("ListGames");
        }

        [HttpGet]
        public ActionResult<IEnumerable<CartItem>> GetAllCartItems()
        {
            if (_userManager.GetUserAsync(User).Result == null)
            {
                return RedirectToAction("BuyGames");
            }

            string username = _userManager.GetUserAsync(User).Result.UserName;
            var cartItems = _service.GetAllCartItemsByUsername(username);
            return View("Cart", cartItems);
        }

        public async Task<IActionResult> IncreaseGameCount(int id)
        {
            string username = _userManager.GetUserAsync(User).Result.UserName;
            var cart = _service.GetAllCartItemsByUsername(username)
                .FirstOrDefault(c => c.Id == id);
            cart.Quantity += 1;

            await _service.UpdateCartAsync(cart);
            return RedirectToAction("GetAllCartItems");
        }

        public async Task<IActionResult> DecreaseGameCount(int id)
        {
            string username = _userManager.GetUserAsync(User).Result.UserName;
            var cart = _service.GetAllCartItemsByUsername(username)
                .FirstOrDefault(c => c.Id == id);

            if (cart.Quantity > 1)
            {
                cart.Quantity -= 1;
                await _service.UpdateCartAsync(cart);
            }
            else
            {
                await _service.RemoveCartItemFromUserAsync(cart);
            }

            return RedirectToAction("GetAllCartItems");
        }

        public async Task<IActionResult> RemoveGameFromCart(int id)
        {
            string username = _userManager.GetUserAsync(User).Result.UserName;
            var cart = _service.GetAllCartItemsByUsername(username)
                .FirstOrDefault(c => c.Id == id);

            await _service.RemoveCartItemFromUserAsync(cart);
            return RedirectToAction("GetAllCartItems");
        }

        [HttpGet]
        public ActionResult<CustomerVM> BuyGames()
        {
            var customer = new CustomerVM();
            return View("BuyGames", customer);
        }

        [HttpPost]
        public async Task<IActionResult> OrderGames(CustomerVM customerVM)
        {
            if (_userManager.GetUserAsync(User).Result == null)
            {
                return RedirectToAction("ListGames");
            }

            string username = _userManager.GetUserAsync(User).Result.UserName;
            await _service.ClearAllCartItemsAsync(username);
            return RedirectToAction("GetAllCartItems");
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
