using backend.Data;
using Data.Entities;
using ErrorOr;
using GameNewsHub.Api.Mappings;
using GameNewsHub.Contracts;
using Microsoft.EntityFrameworkCore;

namespace GameNewsHub.Api.Features.Games;

public class GameService 
{
    private readonly AppDbContext _context;

    public GameService(AppDbContext context)
    {
        _context = context;
    }


    public async Task<List<GameListItemDto>> GetGamesListAsync()
    {
        var games = await _context.Games
            .Where(g => g.Type == GameType.MainGame)
            .Select(g => g.ToListItemDto())
            .Take(100)
            .ToListAsync();
        return games;
    }

    public Task<List<GameListItemDto>> SearchGamesAsync(string query)
    {
        throw new NotImplementedException();
    }

    public async Task<ErrorOr<GameDetailDto>> GetGameDetailsAsync(int gameId)
    {
        var game = await _context.Games
            .FirstOrDefaultAsync(g => g.Id == gameId);

        if (game == null)
        {
            return Error.NotFound("Game.NotFound", $"Game with id {gameId} not found.");
        }
        
        return game.ToDetailDto();
    }

    public async Task<ErrorOr<List<GameListItemDto>>> GetGameAddons(int gameId)
    {
        var gameAddons = await _context.Games
            .Where(g => g.ParentGameId == gameId)
            .Select(g => g.ToListItemDto())
            .ToListAsync();

        if (gameAddons.Count == 0)
        {
            return Error.NotFound("GameAddons.NotFound", $"GameAddons not found.");
        }

        return gameAddons;
    }
}