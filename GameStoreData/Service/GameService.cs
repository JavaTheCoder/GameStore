using GameStoreData.Identity.Data;
using GameStoreData.Models;
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

        public async Task CreateGameAsync(Game game)
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
            await _context.SaveChangesAsync();
        }
    }
}
