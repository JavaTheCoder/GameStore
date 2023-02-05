using GameStoreData.Identity.Data;
using GameStoreData.Models;
using GameStoreData.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GameStoreData.Service
{
    public class GameService
    {
        private readonly ApplicationDbContext _context;

        public GameService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Game> GetGameByIdAsync(int id)
        {
            return await _context.Games.Include(g => g.Genres).FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<List<Game>> GetGamesAsync()
        {
            return await _context.Games.Include(g => g.Genres).ToListAsync();
        }

        public async Task DeleteGameAsync(int id)
        {
            var game = await GetGameByIdAsync(id);
            var gameComments = _context.Comments.Where(c => c.GameId == game.Id);
            _context.Games.Remove(game);
            _context.Comments.RemoveRange(gameComments);
            await _context.SaveChangesAsync();
        }

        public async Task<Game> CreateGameAsync(GameVM gameVM)
        {
            var game = new Game
            {
                Id = gameVM.Id,
                Name = gameVM.Name,
                Genres = gameVM.Genres,
                Image = gameVM.Image,
                Price = gameVM.Price,
                CommentVM = new CommentVM()
            };

            foreach (int id in gameVM.SelectedGenreIds)
            {
                game.Genres.Add(await GetGenreById(id));
            }

            return game;
        }

        public async Task CreateNewGameAsync(Game game)
        {
            _context.Games.Add(game);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateGameAsync(Game game)
        {
            var gameToUpdate = await GetGameByIdAsync(game.Id);
            gameToUpdate.Name = game.Name;
            gameToUpdate.Price = game.Price;
            gameToUpdate.Image = game.Image;

            foreach (var genre in gameToUpdate.Genres.ToList())
            {
                if (!game.Genres.Contains(genre))
                {
                    gameToUpdate.Genres.Remove(genre);
                }
            }

            foreach (var genre in game.Genres.ToList())
            {
                if (!gameToUpdate.Genres.Contains(genre))
                {
                    gameToUpdate.Genres.Add(genre);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<Genre>> GetGenresAsync()
        {
            return await _context.Genres.ToListAsync();
        }

        public async Task<List<Genre>> GetGenresAsSelectListAsync()
        {
            return await _context.Genres.ToListAsync();
        }

        public async Task<Genre> GetGenreById(int id)
        {
            return await _context.Genres.Include(g => g.Games).FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<GameVM> CreateGameVMAsync(Game game)
        {
            var gameVM = new GameVM
            {
                Id = game.Id,
                Image = game.Image,
                Name = game.Name,
                Price = game.Price,
                Genres = game.Genres
            };

            var list = new List<SelectListItem>();
            foreach (var genre in await GetGenresAsync())
            {
                list.Add(new SelectListItem()
                {
                    Text = genre.Name,
                    Value = genre.Id.ToString(),
                });
            }

            gameVM.GenresList = list;
            gameVM.SelectedGenreIds = game.Genres.Select(g => g.Id).ToList();
            return gameVM;
        }

        public async Task SaveCartAsync(CartItem cartItem)
        {
            _context.Add(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCartAsync(CartItem cartItem)
        {
            var oldCart = GetCartByIdAndUsername(cartItem.Username, cartItem.Id);
            oldCart.Quantity = cartItem.Quantity;
            await _context.SaveChangesAsync();
        }

        public CartItem GetCartByIdAndUsername(string userName, int id)
        {
            return _context.UsersCartItems.Include(c => c.GameInCart)
                .Where(c => c.Username == userName).FirstOrDefault(c => c.Id == id);
        }

        public IEnumerable<CartItem> GetAllCartItemsByUsername(string userName)
        {
            return _context.UsersCartItems.Include(c => c.GameInCart).Where(c => c.Username == userName);
        }

        public async Task RemoveCartItemFromUserAsync(CartItem cart)
        {
            _context.UsersCartItems.Remove(cart);
            await _context.SaveChangesAsync();
        }

        public async Task ClearAllCartItemsAsync(string userName)
        {
            // TODO: Clear CartItems when user purchased
            var cart = _context.UsersCartItems.Where(c => c.Username == userName);
            if (cart != null)
            {
                cart.ToList().RemoveRange(0, cart.Count());
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddCommentAsync(CommentVM commentVM, string userId)
        {
            var comment = new Comment
            {
                Body = commentVM.Body,
                GameId = commentVM.GameId,
                TimeLeft = DateTime.Now,
                UserId = userId,
                ParentCommentId = commentVM.ParentCommentId
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCommentAsync(CommentVM commentVM)
        {
            var comment = _context.Comments.FirstOrDefault(c => c.Id == commentVM.CommentVMId);
            comment.Body = commentVM.Body;
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Comment>> LoadGameCommentsById(int id)
        {
            return await _context.Comments.Where(c => c.GameId == id).ToListAsync();
        }

        public async Task DeleteCommentAsync(Comment comment)
        {
            if (comment.ChildComments != null)
            {
                _context.Comments.RemoveRange(comment.ChildComments);
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }

        public async Task<Comment> GetCommentByIdAsync(int id)
        {
            return await _context.Comments.Include(c => c.ChildComments).FirstOrDefaultAsync(c => c.Id == id);
        }

        public GameVM InitializeGenresList(GameVM gameVM, List<Genre> genres)
        {
            var list = new List<SelectListItem>();
            foreach (var genre in genres)
            {
                list.Add(new SelectListItem()
                {
                    Text = genre.Name,
                    Value = genre.Id.ToString(),
                });
            }

            gameVM.GenresList = list;
            return gameVM;
        }
    }
}
