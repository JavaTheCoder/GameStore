using GameStoreData.Identity.Data;
using GameStoreData.Models;
using GameStoreData.Service;
using GameStoreData.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GameStoreWeb.Controllers
{
    public class CartItemController : Controller
    {
        private readonly IGameService _service;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartItemController(IGameService service, UserManager<ApplicationUser> userManager)
        {
            _service = service;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> AddGameToCart(int id)
        {
            string username = _userManager.GetUserName(User);
            var game = await _service.GetGameByIdAsync(id);

            var cartItem = _service.GetAllCartItemsByUsername(username)
                .FirstOrDefault(c => c?.GameInCart?.Name == game.Name);

            if (cartItem != null)
            {
                cartItem.Quantity += 1;
                await _service.UpdateCartItemAsync(cartItem);
                return RedirectToAction("ListGames", "Game");
            }

            await _service.AddNewCartItemAsync(
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

            string username = _userManager.GetUserName(User);
            var cartItems = _service.GetAllCartItemsByUsername(username);
            return View("CartItem", cartItems);
        }

        public async Task<IActionResult> IncreaseGameCount(int id)
        {
            string username = _userManager.GetUserName(User);
            var cartItem = _service.GetAllCartItemsByUsername(username)
                .FirstOrDefault(c => c.Id == id);
            cartItem.Quantity += 1;

            await _service.UpdateCartItemAsync(cartItem);
            return RedirectToAction("GetAllCartItems");
        }

        public async Task<IActionResult> DecreaseGameCount(int id)
        {
            string username = _userManager.GetUserName(User);
            var cartItem = _service.GetAllCartItemsByUsername(username)
                .FirstOrDefault(c => c.Id == id);

            if (cartItem.Quantity > 1)
            {
                cartItem.Quantity -= 1;
                await _service.UpdateCartItemAsync(cartItem);
            }
            else
            {
                await _service.RemoveCartItemFromUserAsync(cartItem);
            }

            return RedirectToAction("GetAllCartItems");
        }

        public async Task<IActionResult> RemoveCartItem(int id)
        {
            string username = _userManager.GetUserName(User);

            var cart = _service.GetAllCartItemsByUsername(username)
                .FirstOrDefault(c => c.Id == id);

            await _service.RemoveCartItemFromUserAsync(cart);
            return RedirectToAction("GetAllCartItems");
        }

        [HttpGet]
        public ActionResult<CustomerVM> BuyGames()
        {
            var customerVM = new CustomerVM();
            string username = _userManager.GetUserName(User);
            if (username == null)
            {
                return View("BuyGames", customerVM);
            }

            if (!_service.GetAllCartItemsByUsername(username).Any())
            {
                return RedirectToAction("ListGames", "Game");
            }

            return View("BuyGames", customerVM);
        }

        [HttpPost]
        public async Task<IActionResult> OrderGames(CustomerVM customerVM)
        {
            string username = _userManager.GetUserName(User);
            if (username != null)
            {
                await _service.ClearAllCartItemsAsync(username);
            }

            return RedirectToAction("ListGames", "Game");
        }
    }
}
