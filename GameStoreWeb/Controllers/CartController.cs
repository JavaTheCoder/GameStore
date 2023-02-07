using GameStoreData.Identity.Data;
using GameStoreData.Models;
using GameStoreData.Service;
using GameStoreData.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GameStoreWeb.Controllers
{
    public class CartController : Controller
    {
        private readonly GameService _service;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(GameService service, UserManager<ApplicationUser> userManager)
        {
            _service = service;
            _userManager = userManager;
        }

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
                return RedirectToAction("ListGames", "Game");
            }

            await _service.SaveCartAsync(
                new CartItem
                {
                    Quantity = 1,
                    GameInCart = game,
                    Username = username
                }
            );
            return RedirectToAction("ListGames", "Game");
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
            var customerVM = new CustomerVM();
            if (_userManager.GetUserName(User) == null)
            {
                return View("BuyGames", customerVM);
            }

            if (!_service.GetAllCartItemsByUsername(_userManager.GetUserName(User)).Any())
            {
                return RedirectToAction("ListGames", "Game");
            }

            return View("BuyGames", customerVM);
        }

        [HttpPost]
        public async Task<IActionResult> OrderGames(CustomerVM customerVM)
        {
            if (_userManager.GetUserAsync(User).Result == null)
            {
                return RedirectToAction("ListGames", "Game");
            }

            string username = _userManager.GetUserAsync(User).Result.UserName;
            await _service.ClearAllCartItemsAsync(username);
            return RedirectToAction("ListGames", "Game");
        }
    }
}
