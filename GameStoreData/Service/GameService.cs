using GameStoreData.Identity.Data;
using GameStoreData.Models;
using GameStoreData.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

#nullable disable
namespace GameStoreData.Service
{
    public class GameService
    {
        private readonly ApplicationDbContext _context;

        public GameService()
        {
        }

        public GameService(ApplicationDbContext context)
        {
            _context = context;
        }

        public virtual async Task<Game> GetGameByIdAsync(int id)
        {
            return await _context.Games
                .Include(g => g.Genres)
                .FirstOrDefaultAsync(g => g.Id == id);
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
                game.Genres.Add(await GetGenreByIdAsync(id));
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

        public async Task<Genre> GetGenreByIdAsync(int id)
        {
            return await _context.Genres
                .Include(g => g.Games)
                .FirstOrDefaultAsync(g => g.Id == id);
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

            gameVM = InitializeGenresList(gameVM, await GetGenresAsync());
            gameVM.SelectedGenreIds = game.Genres.Select(g => g.Id).ToList();

            return gameVM;
        }

        public async Task<ICollection<Game>> GetGamesWithSelectedGenresAsync(ICollection<int> selectedGenresIds)
        {
            return await _context.Games
                .AsNoTracking()
                .Include(g => g.Genres)
                .Where(g => g.Genres
                .Any(g => selectedGenresIds
                .Contains(g.Id)))
                .ToListAsync();
        }

        public GameVM InitializeGenresList(GameVM gameVM, List<Genre> genres)
        {
            gameVM.GenresList = genres.Select(g =>
                new SelectListItem(g.Name, g.Id.ToString()));

            return gameVM;
        }

        public virtual async Task AddNewCartAsync(CartItem cartItem)
        {
            _context.Add(cartItem);
            await _context.SaveChangesAsync();
        }

        public virtual async Task UpdateCartAsync(CartItem cartItem)
        {
            GetOldCartWithUpdatedQuantity(cartItem);
            await _context.SaveChangesAsync();
        }

        public CartItem GetOldCartWithUpdatedQuantity(CartItem cartItem)
        {
            var oldCart = GetCartByIdAndUsername(cartItem.Username, cartItem.Id);
            oldCart.Quantity = cartItem.Quantity;
            return oldCart;
        }

        public CartItem GetCartByIdAndUsername(string userName, int id)
        {
            return _context.UsersCartItems
                .Include(c => c.GameInCart)
                .Where(c => c.Username == userName)
                .FirstOrDefault(c => c.Id == id);
        }

        public virtual IEnumerable<CartItem> GetAllCartItemsByUsername(string userName)
        {
            return _context.UsersCartItems
                .Include(c => c.GameInCart)
                .Where(c => c.Username == userName);
        }

        public async Task RemoveCartItemFromUserAsync(CartItem cart)
        {
            _context.UsersCartItems.Remove(cart);
            await _context.SaveChangesAsync();
        }

        public async Task ClearAllCartItemsAsync(string userName)
        {
            var cart = _context.UsersCartItems.Where(c => c.Username == userName);
            if (cart != null)
            {
                foreach (var item in _context.UsersCartItems)
                {
                    _context.UsersCartItems.Remove(item);
                }
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

        public virtual async Task DeleteInactiveComments(IEnumerable<Comment> comments)
        {
            var inactiveComments = comments.Where(c => !c.IsActive);
            foreach (var c in inactiveComments)
            {
                await DeleteCommentAsync(c);
            }
        }

        public async Task<Game> GetGameAndDeleteInactiveComments(bool isRedirected, int gameId)
        {
            var game = await GetGameByIdAsync(gameId);
            var comments = await LoadGameCommentsById(gameId);

            if (!isRedirected)
            {
                await DeleteInactiveComments(comments);
            }

            game.CommentVM = new CommentVM
            {
                GameId = game.Id,
                GameComments = comments
            };

            return game;
        }

        public virtual async Task<ICollection<Comment>> LoadGameCommentsById(int id)
        {
            return await _context.Comments.Where(c => c.GameId == id).ToListAsync();
        }

        public async Task DeleteCommentAsync(Comment comment)
        {
            if (comment.ChildComments != null)
            {
                var children = await _context.Comments
                    .Include(c => c.ChildComments)
                    .Where(c => c.ParentCommentId == comment.Id)
                    .ToListAsync();

                foreach (var child in children)
                {
                    await DeleteCommentAsync(child);
                }
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }

        public async Task<Comment> GetCommentByIdAsync(int id)
        {
            return await _context.Comments
                .Include(c => c.ChildComments)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task ChangeCommentStateAsync(Comment comment)
        {
            comment.IsActive = !comment.IsActive;
            await _context.SaveChangesAsync();
        }
    }
}
