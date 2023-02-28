using GameStoreData.Models;
using GameStoreData.Repository;
using GameStoreData.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

#nullable disable
namespace GameStoreData.Service
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _repository;

        public GameService(IGameRepository repository)
        {
            _repository = repository;
        }

        public virtual async Task<Game> GetGameByIdAsync(int id)
        {
            return await _repository.GetGameByIdAsync(id);
        }

        public async Task<List<Game>> GetGamesAsync()
        {
            return await _repository.GetAllGamesAsync();
        }

        public async Task DeleteGameAsync(int id)
        {
            await _repository.DeleteGameAsync(id);
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

        public async Task CreateNewGameAsync(Game game)
        {
            await _repository.CreateNewGameAsync(game);
        }

        public async Task UpdateGameAsync(Game game)
        {
            var gameToUpdate = await _repository.GetGameByIdAsync(game.Id);
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

            await _repository.UpdateGameAsync();
        }

        public async Task<List<Genre>> GetGenresAsync()
        {
            return await _repository.GetAllGenresAsync();
        }

        public async Task<Genre> GetGenreByIdAsync(int id)
        {
            return await _repository.GetGenreByIdAsync(id);
        }

        public async Task<ICollection<Game>> GetGamesWithSelectedGenresAsync(ICollection<int> selectedGenresIds)
        {
            return await _repository.GetGamesWithSelectedGenresAsync(selectedGenresIds);
        }

        public GameVM InitializeGenresList(GameVM gameVM, List<Genre> genres)
        {
            gameVM.GenresList = genres.Select(g =>
                new SelectListItem(g.Name, g.Id.ToString()));

            return gameVM;
        }

        public virtual async Task AddNewCartItemAsync(CartItem cartItem)
        {
            await _repository.AddNewCartAsync(cartItem);
        }

        public virtual async Task<CartItem> UpdateCartItemAsync(CartItem updatedCart)
        {
            return await _repository.UpdateCartAsync(updatedCart);
        }

        public virtual CartItem GetCartItemByIdAndUsername(string userName, int id)
        {
            return _repository.GetCartItemByIdAndUsername(userName, id);
        }

        public virtual IEnumerable<CartItem> GetAllCartItemsByUsername(string userName)
        {
            return _repository.GetAllCartItemsByUsername(userName);
        }

        public async Task RemoveCartItemFromUserAsync(CartItem cart)
        {
            await _repository.RemoveCartItemFromUserAsync(cart);
        }

        public async Task ClearAllCartItemsAsync(string userName)
        {
            await _repository.ClearAllCartItemsAsync(userName);
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

            await _repository.AddCommentAsync(comment);
        }

        public async Task UpdateCommentAsync(CommentVM commentVM)
        {
            await _repository.UpdateCommentAsync(commentVM);
        }

        public async Task<Game> GetGameAndDeleteInactiveComments(bool isRedirected, int gameId)
        {
            var game = await GetGameByIdAsync(gameId);
            var comments = await LoadGameCommentsById(gameId);

            if (!isRedirected)
            {
                await _repository.DeleteInactiveComments(comments);
            }

            game.CommentVM = new CommentVM
            {
                GameId = game.Id,
                GameComments = await LoadGameCommentsById(gameId)
            };

            return game;
        }

        public virtual async Task<ICollection<Comment>> LoadGameCommentsById(int id)
        {
            return await _repository.LoadGameCommentsById(id);
        }

        public async Task DeleteCommentAsync(Comment comment)
        {
            await _repository.DeleteCommentAsync(comment);
        }

        public async Task<Comment> GetCommentByIdAsync(int id)
        {
            return await _repository.GetCommentByIdAsync(id);
        }

        public async Task ChangeCommentStateAsync(Comment comment)
        {
            await _repository.ChangeCommentStateAsync(comment);
        }
    }
}
