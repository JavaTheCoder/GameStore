using GameStoreData.Identity.Data;
using GameStoreData.Service;
using GameStoreData.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GameStoreWeb.Controllers
{
    public class CommentController : Controller
    {
        private readonly GameService _service;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentController(GameService service, UserManager<ApplicationUser> userManager)
        {
            _service = service;
            _userManager = userManager;
        }

        public async Task<IActionResult> ReplyToComment(int id)
        {
            var comment = await _service.GetCommentByIdAsync(id);
            var game = await _service.GetGameByIdAsync(comment.GameId);

            game.CommentVM = new CommentVM
            {
                GameId = game.Id,
                ParentCommentId = id
            };

            return View("Views/Game/Details.cshtml", game);
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
            return RedirectToAction("Details", "Game", new { id = commentVM.GameId });
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
                //await _service.DeleteCommentAsync(comment);
                //comment.IsActive = false;
            }

            return RedirectToAction("Details", "Game", new { id = comment.GameId });
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

            return View("Views/Game/Details.cshtml", game);
        }
    }
}
