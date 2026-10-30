using ErrorOr;
using GameNewsHub.Api.Configuration;
using GameNewsHub.Api.Features.Platforms;
using GameNewsHub.Api.Mappings;
using GameNewsHub.Contracts;
using GameNewsHub.Contracts.Games;
using GameNewsHub.Contracts.Shared;
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

    public async Task<SearchDto<GameListItemDto>> SearchGamesAsync(
        string? query, int page, int pageSize,
        GameSortBy? sortBy, bool descending)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        IQueryable<Game> filteredQuery = _context.Games
            .Where(g => GameTypePolicy.MainTypes.Contains(g.Type));

        var cleanQuery = query?.Trim();
        var hasQuery = !string.IsNullOrWhiteSpace(query);

        string like = "", exact = "", prefix = "", wordStart = "";
        if (hasQuery)
        {
            exact = cleanQuery;
            prefix = cleanQuery + "%";
            wordStart = "% " + cleanQuery + "%";
            like = "%" + cleanQuery + "%";

            filteredQuery = filteredQuery.Where(g =>
                EF.Functions.ILike(g.Title, like) ||
                EF.Functions.TrigramsAreWordSimilar(g.Title, cleanQuery));
        }

        var total = await filteredQuery.CountAsync();

        IQueryable<Game> sortedQuery;
        if (!hasQuery)
        {
            sortedQuery = filteredQuery.ApplySorting(sortBy ?? GameSortBy.Name, descending);
        }
        else if (sortBy.HasValue && sortBy.Value != GameSortBy.Relevance)
        {
            sortedQuery = filteredQuery.ApplySorting(sortBy.Value, descending);
        }
        else
        {
            sortedQuery = filteredQuery
                .OrderBy(g =>
                    EF.Functions.ILike(g.Title, exact) ? 0 :
                    EF.Functions.ILike(g.Title, prefix) ? 1 :
                    EF.Functions.ILike(g.Title, wordStart) ? 2 :
                    EF.Functions.ILike(g.Title, like) ? 3 : 4)
                .ThenByDescending(g => EF.Functions.TrigramsWordSimilarity(cleanQuery, g.Title))
                .ThenByDescending(g => g.FollowerCount);
        }

        var items = await sortedQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(g => g.ToListItemDto())
            .ToListAsync();

        return new SearchDto<GameListItemDto>
        {
            Total = total,
            Items = items
        };
    }


    public async Task<ErrorOr<GameDetailDto>> GetGameDetailsAsync(int gameId)
    {
        // it's slower with split query
        var game = await _context.Games
            .AsSingleQuery()
            .Include(g => g.ChildGames)
            .Include(g => g.ParentGame)
            .Include(g => g.Platforms)
            .ThenInclude(pg => pg.PlatformGroup)
            .Include(g => g.Genres)
            .FirstOrDefaultAsync(g => g.Id == gameId);

        if (game == null) return Error.NotFound("Game.NotFound", $"Game with id {gameId} not found.");

        // ???
        var platformGroups = game.Platforms
            .GroupBy(p => p.PlatformGroup)
            .Select(pg =>
            {
                var platforms = pg
                    .Select(p => p.ToDto())
                    .ToList();

                return pg.Key.ToDtoWithPlatforms(platforms);
            })
            .ToList();

        var genres = game.Genres
            .Select(g => g.ToDto())
            .ToList();
        var addons = game.ChildGames
            .Select(g => g.ToListItemDto())
            .ToList();
        var parent = game.ParentGame;
        var parentGame = parent is null
            ? null
            : GameMappingExtension.ToParentGameDto(parent);

        return game.ToDetailDto(genres, addons, platformGroups, parentGame);
    }

    public async Task<List<GameListItemDto>> GetGameAddons(int gameId)
    {
        var gameAddons = await _context.Games
            .Where(g => g.ParentGameId == gameId)
            .ProjectToListItemDto()
            .ToListAsync();

        return gameAddons;
    }
}