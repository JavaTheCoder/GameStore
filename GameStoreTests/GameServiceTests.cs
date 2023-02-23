using GameStoreData.Identity.Data;
using GameStoreData.Models;
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
        private Mock<UserManager<ApplicationUser>> _userManager;
        private CartController _controller;

        [SetUp]
        public void Setup()
        {
            _service = new Mock<GameService>();
            _service.CallBase = true;

            _userManager = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
            _controller = new CartController(_service.Object, _userManager.Object);

            _service.Setup(s => s.GetGameByIdAsync(It.IsAny<int>())).ReturnsAsync(new Game
            {
                Id = 1,
                Name = "Test Game",
                Price = 49.99
            });

            _service.Setup(s => s.LoadGameCommentsById(It.IsAny<int>())).ReturnsAsync(new List<Comment>
            {
                new Comment { Id = 1, Body = "Test Comment" }
            });

            _service.Setup(s => s.AddNewCartAsync(It.IsAny<CartItem>()));

            _service.Setup(s => s.UpdateCartAsync(It.IsAny<CartItem>()));

            _service.Setup(s => s.DeleteInactiveComments(It.IsAny<ICollection<Comment>>()));

            _service.Setup(s => s.GetAllCartItemsByUsername(It.IsAny<string>())).Returns(new List<CartItem>
            {
                new CartItem { Id = 1, Username = "barrywhite" }
            });

            _userManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()).Result.UserName).Returns("barrywhite");
        }

        [Test]
        public async Task GetGameAndDeleteInactiveComments_Redirected_DoNotDeleteComments()
        {
            var game = await _service.Object.GetGameAndDeleteInactiveComments(true, 1);
            _service.Verify(s => s.DeleteInactiveComments(It.IsAny<ICollection<Comment>>()), Times.Never);
        }

        [Test]
        public async Task GetGameAndDeleteInactiveComments_NotRedirected_DeleteComments()
        {
            var game = await _service.Object.GetGameAndDeleteInactiveComments(false, 1);
            _service.Verify(s => s.DeleteInactiveComments(It.IsAny<ICollection<Comment>>()), Times.Once);
        }

        [Test]
        public async Task AddToCart_CartExists_UpdateQuantity()
        {
            await _controller.AddToCart(1);
            _service.Verify(s => s.UpdateCartAsync(It.IsAny<CartItem>()), Times.Once);
        }

        [Test]
        public async Task AddToCart_CartDoesNotExist_AddNewCart()
        {
            await _controller.AddToCart(15);
            _service.Verify(s => s.UpdateCartAsync(It.IsAny<CartItem>()), Times.Once);
        }
    }
}