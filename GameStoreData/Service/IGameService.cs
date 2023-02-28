using GameStoreData.Models;
using GameStoreData.ViewModels;

namespace GameStoreData.Service
{
    public interface IGameService
    {
        public Task<Game> GetGameByIdAsync(int id);

        public Task<List<Game>> GetGamesAsync();

        public Task DeleteGameAsync(int id);

        public Task<Game> CreateGameAsync(GameVM gameVM);

        public Task<GameVM> CreateGameVMAsync(Game game);

        public Task CreateNewGameAsync(Game game);

        public Task UpdateGameAsync(Game game);

        public Task<List<Genre>> GetGenresAsync();

        public Task<Genre> GetGenreByIdAsync(int id);

        public Task<ICollection<Game>> GetGamesWithSelectedGenresAsync(ICollection<int> selectedGenresIds);

        public GameVM InitializeGenresList(GameVM gameVM, List<Genre> genres);

        public Task AddNewCartItemAsync(CartItem cartItem);

        public Task<CartItem> UpdateCartItemAsync(CartItem updatedCart);

        public CartItem GetCartItemByIdAndUsername(string userName, int id);

        public IEnumerable<CartItem> GetAllCartItemsByUsername(string userName);

        public Task RemoveCartItemFromUserAsync(CartItem cart);

        public Task ClearAllCartItemsAsync(string userName);

        public Task AddCommentAsync(CommentVM commentVM, string userId);

        public Task UpdateCommentAsync(CommentVM commentVM);

        public Task<Game> GetGameAndDeleteInactiveComments(bool isRedirected, int gameId);

        public Task<ICollection<Comment>> LoadGameCommentsById(int id);

        public Task DeleteCommentAsync(Comment comment);

        public Task<Comment> GetCommentByIdAsync(int id);

        public Task ChangeCommentStateAsync(Comment comment);
    }
}
