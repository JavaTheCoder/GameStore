using GameStoreData.Models;
using GameStoreData.ViewModels;

namespace GameStoreData.Repository
{
    public interface IGameRepository
    {
        // -- GAME --
        Task<Game> GetGameByIdAsync(int id);

        Task<List<Game>> GetAllGamesAsync();

        Task DeleteGameAsync(int id);

        Task CreateNewGameAsync(Game game);

        Task UpdateGameAsync();

        Task<List<Genre>> GetAllGenresAsync();

        Task<Genre> GetGenreByIdAsync(int id);

        Task<ICollection<Game>> GetGamesWithSelectedGenresAsync(ICollection<int> selectedGenresIds);

        // -- CART --
        Task AddNewCartAsync(CartItem cartItem);

        CartItem GetCartItemByIdAndUsername(string userName, int id);

        IEnumerable<CartItem> GetAllCartItemsByUsername(string userName);

        Task<CartItem> UpdateCartAsync(CartItem updatedCart);

        Task RemoveCartItemFromUserAsync(CartItem cart);

        Task ClearAllCartItemsAsync(string userName);

        // -- COMMENT --
        Task AddCommentAsync(Comment comment);

        Task UpdateCommentAsync(CommentVM commentVM);

        Task DeleteInactiveComments(IEnumerable<Comment> comments);

        Task<ICollection<Comment>> LoadGameCommentsById(int id);

        Task DeleteCommentAsync(Comment comment);

        Task<Comment> GetCommentByIdAsync(int id);

        Task ChangeCommentStateAsync(Comment comment);
    }
}
