using GameStoreData.Models;
using GameStoreData.ViewModels;

namespace GameStoreData.Service
{
    public interface IGameService
    {
        Task<Game> GetGameByIdAsync(int id);

        Task<List<Game>> GetGamesAsync();

        Task DeleteGameAsync(int id);

        Task<Game> CreateGameAsync(GameVM gameVM);

        Task<GameVM> CreateGameVMAsync(Game game);

        Task CreateNewGameAsync(Game game);

        Task UpdateGameAsync(Game game);

        Task<List<Genre>> GetGenresAsync();

        Task<Genre> GetGenreByIdAsync(int id);

        Task<ICollection<Game>> GetGamesWithSelectedGenresAsync(ICollection<int> selectedGenresIds);

        GameVM InitializeGenresList(GameVM gameVM, List<Genre> genres);

        Task AddNewCartItemAsync(CartItem cartItem);

        Task<CartItem> UpdateCartItemAsync(CartItem updatedCart);

        CartItem GetCartItemByIdAndUsername(string userName, int id);

        IEnumerable<CartItem> GetAllCartItemsByUsername(string userName);

        Task RemoveCartItemFromUserAsync(CartItem cart);

        Task ClearAllCartItemsAsync(string userName);

        Task AddCommentAsync(CommentVM commentVM, string userId);

        Task UpdateCommentAsync(CommentVM commentVM);

        Task<Game> GetGameAndDeleteInactiveComments(bool isRedirected, int gameId);

        Task<ICollection<Comment>> LoadGameCommentsById(int id);

        Task DeleteCommentAsync(Comment comment);

        Task<Comment> GetCommentByIdAsync(int id);

        Task ChangeCommentStateAsync(Comment comment);
    }
}
