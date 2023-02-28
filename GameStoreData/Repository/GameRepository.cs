using GameStoreData.Identity.Data;
using GameStoreData.Models;
using GameStoreData.ViewModels;
using Microsoft.EntityFrameworkCore;

#nullable disable
namespace GameStoreData.Repository
{
    public class GameRepository : IGameRepository
    {
        private readonly ApplicationDbContext _context;

        public GameRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public virtual async Task<Game> GetGameByIdAsync(int id)
        {
            return await _context.Games
                .Include(g => g.Genres)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<List<Game>> GetAllGamesAsync()
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

        public async Task CreateNewGameAsync(Game game)
        {
            _context.Games.Add(game);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateGameAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<Genre>> GetAllGenresAsync()
        {
            return await _context.Genres.ToListAsync();
        }

        public async Task<Genre> GetGenreByIdAsync(int id)
        {
            return await _context.Genres
                .Include(g => g.Games)
                .FirstOrDefaultAsync(g => g.Id == id);
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

        public virtual async Task AddNewCartAsync(CartItem cartItem)
        {
            _context.Add(cartItem);
            await _context.SaveChangesAsync();
        }

        public CartItem GetCartItemByIdAndUsername(string userName, int id)
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
            var cartItems = _context.UsersCartItems.Where(c => c.Username == userName);
            _context.UsersCartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
        }

        public async Task AddCommentAsync(Comment comment)
        {
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
            var inactiveComments = comments.Where(c => c.IsActive == false);
            foreach (var comment in inactiveComments)
            {
                await DeleteCommentAsync(comment);
            }
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

        public async Task<CartItem> UpdateCartAsync(CartItem updatedCart)
        {
            var cartItem = GetCartItemByIdAndUsername(updatedCart.Username, updatedCart.Id);
            cartItem.Quantity = updatedCart.Quantity;
            await _context.SaveChangesAsync();
            return cartItem;
        }
    }
}
