using ErrorOr;
using GameNewsHub.Api.Configuration;
using GameNewsHub.Api.Features.Platforms;
using GameNewsHub.Api.Mappings;
using GameNewsHub.Contracts;
using GameNewsHub.Data;
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
        var baseQuery = _context.Games
            .Where(g => GameTypePolicy.MainTypes.Contains(g.Type));
        
        if (string.IsNullOrWhiteSpace(query))
        {
            return await baseQuery
                .OrderBy(g => g.Title)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(g => g.ToListItemDto())
                .ToListAsync();
        }

        var cleanQuery = query.Trim();

        var games = await baseQuery
            .Where(g => EF.Functions.ILike(g.Title, $"%{cleanQuery}%")
                || EF.Functions.TrigramsSimilarity(g.Title, $"%{cleanQuery}%") >= 0.25)
            
            .Select(g => new
            {
                Game = g,
                IsExact = EF.Functions.ILike(g.Title, $"%{cleanQuery}%"),
                TrigramScore = EF.Functions.TrigramsSimilarity(g.Title, $"%{cleanQuery}%"),
                EditDistance = EF.Functions.FuzzyStringMatchLevenshteinLessEqual(g.Title, cleanQuery, 3)
            })
            .OrderByDescending(x => x.IsExact)
            .ThenByDescending(x => x.TrigramScore)
            .ThenBy(x => x.EditDistance)
            
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => x.Game.ToListItemDto())
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

        if (game == null) return Error.NotFound("Game.NotFound", $"Game with id {gameId} not found.");

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

        if (gameAddons.Count == 0) return Error.NotFound("GameAddons.NotFound", "GameAddons not found.");

        return gameAddons;
    }
}