using GameStoreData.Identity.Data;
using GameStoreData.Models;
using GameStoreData.Repository;
using GameStoreData.Service;
using GameStoreWeb.Controllers;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Security.Claims;

namespace GameStoreTests
{
    public class GameServiceTests
    {
        private Mock<GameService> _service;
        private Mock<IGameRepository> _repository;
        private Mock<UserManager<ApplicationUser>> _userManager;

        private UserController _userController;
        private CartItemController _cartItemController;

        private Game _testGame;
        private CartItem _testCartItem;
        private Comment _testComment;

        [SetUp]
        public void Setup()
        {
            _repository = new Mock<IGameRepository>();
            _service = new Mock<GameService>(_repository.Object);
            _service.CallBase = true;

            _userManager = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(),
                null, null, null, null, null, null, null, null);
            _cartItemController = new CartItemController(_service.Object, _userManager.Object);
            _userController = new UserController(_userManager.Object);

            _testGame = new Game
            {
                Id = 1,
                Name = "Test Game",
                Price = 49.99
            };
            _testCartItem = new CartItem
            {
                Id = 1,
                Username = "barrywhite",
                GameInCart = _testGame
            };
            _testComment = new Comment
            {
                Id = 1,
                Body = "Test Comment"
            };

            _repository.Setup(r => r.GetGameByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_testGame);

            _service.Setup(s => s.LoadGameCommentsById(It.IsAny<int>()))
                .ReturnsAsync(new List<Comment> { _testComment });

            _service.Setup(s => s.AddNewCartItemAsync(It.IsAny<CartItem>()));

            _service.Setup(s => s.UpdateCartItemAsync(It.IsAny<CartItem>()));

            _repository.Setup(r => r.DeleteInactiveComments(It.IsAny<ICollection<Comment>>()));

            _userManager.Setup(u => u.GetUserName(It.IsAny<ClaimsPrincipal>()));

            _userManager.Setup(u => u.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string> { "User" });
        }

        [Test]
        public async Task GetGameAndDeleteInactiveComments_Redirected_DoNotDeleteComments()
        {
            var game = await _service.Object.GetGameAndDeleteInactiveComments(true, 1);
            _repository.Verify(r => r.DeleteInactiveComments(It.IsAny<ICollection<Comment>>()), Times.Never);
        }

        [Test]
        public async Task ElevateUserToManager_RoleChangesToManager()
        {
            await _userController.ElevateUserToManager("johnsnow");
            _userManager.Verify(u => u.RemoveFromRolesAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<string>>()), Times.Once);
            _userManager.Verify(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task ElevateUserToAdmin_RoleChangesToAdmin()
        {
            await _userController.ElevateUserToAdmin("barrywhite");
            _userManager.Verify(u => u.RemoveFromRolesAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<string>>()), Times.Once);
            _userManager.Verify(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task GetGameAndDeleteInactiveComments_NotRedirected_DeleteComments()
        {
            var game = await _service.Object.GetGameAndDeleteInactiveComments(false, 1);
            _repository.Verify(r => r.DeleteInactiveComments(It.IsAny<ICollection<Comment>>()), Times.Once);
        }

        [Test]
        public async Task AddGameToCart_CartExists_UpdateQuantity()
        {
            _service.Setup(s => s.GetAllCartItemsByUsername(It.IsAny<string>()))
                .Returns(new List<CartItem> { _testCartItem });

            _service.Setup(s => s.GetCartItemByIdAndUsername(It.IsAny<string>(),
                It.IsAny<int>())).Returns(_testCartItem);

            await _cartItemController.AddGameToCart(1);
            _service.Verify(s => s.UpdateCartItemAsync(It.IsAny<CartItem>()), Times.Once);
        }

        [Test]
        public async Task AddGameToCart_CartDoesNotExist_AddNewCart()
        {
            await _cartItemController.AddGameToCart(15);
            _service.Verify(s => s.AddNewCartItemAsync(It.IsAny<CartItem>()), Times.Once);
        }
    }
}