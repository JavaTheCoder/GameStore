using GameStoreData.Identity.Data;
using GameStoreData.Models;
using GameStoreData.Service;
using GameStoreData.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GameStoreWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly GameService _service;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(GameService service, UserManager<ApplicationUser> userManager)
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

        // -------------------------Handling Comments (Epic 3)-------------------------
        public async Task<IActionResult> ReplyToComment(int id)
        {
            var comment = await _service.GetCommentByIdAsync(id);
            var game = await _service.GetGameByIdAsync(comment.GameId);

            game.CommentVM = new CommentVM
            {
                GameId = game.Id,
                ParentCommentId = id
            };

            return View("Details", game);
        }

        public async Task<IActionResult> AddComment(CommentVM commentVM)
        {
            if (commentVM.CommentVMId != 0)
            {
                await _service.UpdateCommentAsync(commentVM);
            }
            else
            {
                await _service.AddCommentAsync(commentVM, _userManager.GetUserId(User));
            }
            return RedirectToAction("Details", new { id = commentVM.GameId });

            //var game = await _service.GetGameByIdAsync(commentVM.GameId);
            //game.CommentVM = new CommentVM
            //{
            //    GameId = game.Id,
            //    ParentCommentId = commentVM.ParentCommentId
            //};
            //return View("Details", game);
        }

        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _service.GetCommentByIdAsync(id);

            if (_userManager.GetUserId(User) == comment.UserId
                || _userManager.GetRolesAsync(_userManager.GetUserAsync(User).Result)
                                            .Result.Contains("Manager")
                || _userManager.GetRolesAsync(_userManager.GetUserAsync(User).Result)
                                            .Result.Contains("Admin"))
            {
                await _service.DeleteCommentAsync(comment);
                return RedirectToAction("Details", new { id = comment.GameId });
            }

            throw new Exception("Only Admin/Managers/Users who created their comments can delete them");
        }

        [HttpGet]
        public async Task<IActionResult> UpdateComment(int id)
        {
            var comment = await _service.GetCommentByIdAsync(id);
            var commentVM = new CommentVM
            {
                CommentVMId = comment.Id,
                Body = comment.Body
            };

            var game = await _service.GetGameByIdAsync(comment.GameId);
            game.CommentVM = commentVM;
            game.CommentVM.GameId = game.Id;

            return View("Details", game);
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
        public async Task<ActionResult<CustomerVM>> BuyGames(int id)
        {
            var customer = new CustomerVM();
            return View("BuyGames", customer);
        }

        [HttpPost]
        public async Task<IActionResult> OrderGames(/*CustomerVM customerVM*/)
        {
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
