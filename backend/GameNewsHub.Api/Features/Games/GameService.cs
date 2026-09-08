using backend.Configuration;
using backend.Data;
using Data.Entities;
using ErrorOr;
using GameNewsHub.Api.Features.Platforms;
using GameNewsHub.Api.Mappings;
using GameNewsHub.Contracts;
using GameNewsHub.Data.Entities;
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

    public async Task<List<GameListItemDto>> SearchGamesAsync(string? query, int page, int pageSize)
    {
        IQueryable<Game> dbQuery = _context.Games.Where(g => GameTypePolicy.MainTypes.Contains(g.Type));

        if (!string.IsNullOrWhiteSpace(query))
        {
            dbQuery = dbQuery.Where(g => g.Title.ToLower().Contains(query.ToLower()));
        }
        
        var games = await dbQuery
            .OrderBy(g => g.Title)
            .Select(g => g.ToListItemDto())
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return games;
    }

    public async Task<ErrorOr<GameDetailDto>> GetGameDetailsAsync(int gameId)
    {
        var game = await _context.Games
            .Include(g => g.ChildGames)
            .Include(g => g.Platforms)
                .ThenInclude(pg => pg.PlatformGroup)
            .Include(g => g.Genres)
            .FirstOrDefaultAsync(g => g.Id == gameId);

        if (game == null)
        {
            return Error.NotFound("Game.NotFound", $"Game with id {gameId} not found.");
        }

        // ???
        var platformGroups = game.Platforms
            .GroupBy(p => p.PlatformGroup)
            .Select(pg => pg.Key.ToDtoWithPlatforms())
            .ToList();
        
        return game.ToDetailDto(platformGroups);
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