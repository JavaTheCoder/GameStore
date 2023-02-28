using GameStoreData.Models;
using GameStoreData.ViewModels;

namespace GameStoreData.Repository
{
    public interface IGameRepository
    {
        // -- GAME --
        public Task<Game> GetGameByIdAsync(int id);

        public Task<List<Game>> GetAllGamesAsync();

        public Task DeleteGameAsync(int id);

        public Task CreateNewGameAsync(Game game);

        public Task UpdateGameAsync();

        public Task<List<Genre>> GetAllGenresAsync();

        public Task<Genre> GetGenreByIdAsync(int id);

        public Task<ICollection<Game>> GetGamesWithSelectedGenresAsync(ICollection<int> selectedGenresIds);

        // -- CART --
        public Task AddNewCartAsync(CartItem cartItem);

        public CartItem GetCartItemByIdAndUsername(string userName, int id);

        public IEnumerable<CartItem> GetAllCartItemsByUsername(string userName);

        public Task<CartItem> UpdateCartAsync(CartItem updatedCart);

        public Task RemoveCartItemFromUserAsync(CartItem cart);

        public Task ClearAllCartItemsAsync(string userName);

        // -- COMMENT --
        public Task AddCommentAsync(Comment comment);

        public Task UpdateCommentAsync(CommentVM commentVM);

        public Task DeleteInactiveComments(IEnumerable<Comment> comments);

        public Task<ICollection<Comment>> LoadGameCommentsById(int id);

        public Task DeleteCommentAsync(Comment comment);

        public Task<Comment> GetCommentByIdAsync(int id);

        public Task ChangeCommentStateAsync(Comment comment);
    }
}
