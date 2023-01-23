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
            _context.Games.Remove(game);
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
            return await _context.Genres.FirstOrDefaultAsync(g => g.Id == id);
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
    }
}
