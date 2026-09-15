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

    public async Task<List<GameListItemDto>> SearchGamesAsync(
        string? query, int page, int pageSize,
        GameSortBy? sortBy, bool descending)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Max(pageSize, 1);

        IQueryable<Game> baseQuery = _context.Games
            .Where(g => GameTypePolicy.MainTypes.Contains(g.Type));

        if (string.IsNullOrWhiteSpace(query))
        {
            return await baseQuery
                .ApplySorting(sortBy ?? GameSortBy.Name, descending)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(g => g.ToListItemDto())
                .ToListAsync();
        }

        var cleanQuery = query.Trim();

        IQueryable<Game> filteredQuery = baseQuery
            .Where(g => EF.Functions.TrigramsAreSimilar(g.Title, cleanQuery));

        IQueryable<Game> sortedQuery;

        if (sortBy.HasValue && sortBy.Value != GameSortBy.Relevance)
        {
            sortedQuery = filteredQuery.ApplySorting(sortBy.Value, descending);
        }
        else
        {
            sortedQuery = filteredQuery
                .OrderByDescending(g => EF.Functions.ILike(g.Title, $"%{cleanQuery}%"))
                .ThenByDescending(g => EF.Functions.TrigramsSimilarity(g.Title, cleanQuery))
                .ThenBy(g => EF.Functions.FuzzyStringMatchLevenshteinLessEqual(g.Title, cleanQuery, 3));
        }

        return await sortedQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(g => g.ToListItemDto())
            .ToListAsync();
    }


    public async Task<ErrorOr<GameDetailDto>> GetGameDetailsAsync(int gameId)
    {
        // it's slower with split query
        var game = await _context.Games
            .AsSingleQuery()
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