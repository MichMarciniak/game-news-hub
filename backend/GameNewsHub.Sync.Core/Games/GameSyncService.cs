using GameNewsHub.Data;
using GameNewsHub.Data.Entities;
using GameNewsHub.External.Igdb.Client;
using GameNewsHub.External.Igdb.Dtos;
using GameNewsHub.Sync.Core.Genres;
using GameNewsHub.Sync.Core.Platforms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GameNewsHub.Sync.Core.Games;

public class GameSyncService : IGameSyncService
{
    private readonly IIgdbClient _client;
    private readonly AppDbContext _context;
    private readonly IGenreSyncService _genreService;
    private readonly ILogger<GameSyncService> _logger;
    private readonly IPlatformSyncService _platformService;

    public GameSyncService(
        IIgdbClient client,
        IGenreSyncService genreService,
        IPlatformSyncService platformService,
        AppDbContext context,
        ILogger<GameSyncService> logger
        )
    {
        _client = client;
        _genreService = genreService;
        _platformService = platformService;
        _context = context;
        _logger = logger;
    }

    public async Task<List<Game>> GetOrCreateGamesAsync(List<int> gameIds)
    {
        return await GetOrCreateGamesAsync(gameIds, new HashSet<int>());
    }

    // przeciążenie z visited, chroni przed nieskonczona rekurencja
    // gdyby parent_game tworzył cykl w igdb
    private async Task<List<Game>> GetOrCreateGamesAsync(List<int> gameIds, HashSet<int> visited)
    {
        if (!gameIds.Any()) return new List<Game>();

        var idsToProcess = gameIds.Where(id => visited.Add(id)).ToList();
        if (!idsToProcess.Any()) return new List<Game>();

        // #1 pobierz isniejące gry z bazy
        var existingGames = await _context.Games
            .Include(g => g.Genres)
            .Where(g => idsToProcess.Contains(g.IgdbId))
            .ToListAsync();

        var missingIds = idsToProcess
            .Except(existingGames.Select(g=> g.IgdbId))
            .ToList();
        if (!missingIds.Any()) return existingGames;

        // #2 pobierz brakujące gry z IGDB i zrób słowniki platform i gatunków
        var externalGames = await _client.UpdateMissingGames(missingIds);

        var genres = await ResolveGenresAsync(externalGames);

        var platforms = await ResolvePlatformsAsync(externalGames);

        var newGames = externalGames
            .Select(g => MapToGameEntity(g, genres, platforms))
            .ToList();

        _context.Games.AddRange(newGames);
        await _context.SaveChangesAsync();

        await LinkParentGamesAsync(newGames, visited);
        
        _logger.LogInformation($"Added {newGames.Count} games");
        
       return existingGames.Concat(newGames).ToList();
    }

    private async Task<List<Genre>> ResolveGenresAsync(List<IgdbGameDto> externalGames)
    {
        var allGenres = externalGames
            .SelectMany(g => g.Genres ?? Enumerable.Empty<IgdbGenreResponse>())
            .DistinctBy(g => g.IgdbId)
            .ToList();

        return await _genreService.GetOrCreateBatchAsync(allGenres);
    }

    private async Task<List<Platform>> ResolvePlatformsAsync(List<IgdbGameDto> externalGames)
    {
        var allPlatforms = externalGames
            .SelectMany(g => g.Platforms ?? Enumerable.Empty<IgdbPlatformResponse>())
            .Distinct()
            .ToList();
        
        return await _platformService.GetOrCreateBatchAsync(allPlatforms);
    }

    private Game MapToGameEntity(IgdbGameDto dto, List<Genre> genres, List<Platform> platforms)
    {
        return new Game
        {
            IgdbId = dto.IgdbId,
            Title = dto.Name,
            Summary = dto.Summary,
            CoverUrl = FormatCoverUrl(dto.Cover?.Url),
            Type = MapGameType(dto.GameType),
            ParentGameIgdbId = dto.ParentGame,
            Genres = MapGenres(dto.Genres, genres),
            ReleaseDate = GetMinReleaseDate(dto.ReleaseDates),
            Platforms = MapPlatforms(dto.Platforms, platforms)
        };
    }

    private static GameType MapGameType(IgdbGameTypeDto? dto)
    {
        if (dto == null) return GameType.MainGame;
        return (GameType)dto.IgdbId;
    }

    private static string? FormatCoverUrl(string? rawUrl)
    {
        if (string.IsNullOrWhiteSpace(rawUrl)) return null;

        var bigCoverUrl = rawUrl.Replace("t_thumb", "t_cover_big");
        return bigCoverUrl.StartsWith("//") ? $"https:{bigCoverUrl}" : bigCoverUrl;
    }

    private static DateTimeOffset? GetMinReleaseDate(List<ReleaseDate>? releaseDates)
    {
        var minDate = releaseDates?.MinBy(x => x.Date)?.Date;
        return minDate.HasValue ? DateTimeOffset.FromUnixTimeSeconds(minDate.Value) : null;
    }

    private static List<Genre> MapGenres(List<IgdbGenreResponse>? dtos, List<Genre> genres)
    {
        if (dtos == null) return new List<Genre>();
        var ids = dtos.Select(d => d.IgdbId).ToHashSet();
        return genres.Where(g => ids.Contains(g.IgdbId)).ToList();
    }

    private List<Platform> MapPlatforms(List<IgdbPlatformResponse>? dtos, List<Platform> platforms)
    {
        if (dtos == null) return new List<Platform>();
        var ids = dtos.Select(d => d.IgdbId).ToHashSet();
        return platforms.Where(p => ids.Contains(p.IgdbId)).ToList();
    }

    private async Task LinkParentGamesAsync(List<Game> newGames, HashSet<int> visited)
    {
        var parentIgdbIds = newGames 
            .Where(g => g.ParentGameIgdbId.HasValue)
            .Select(g => g.ParentGameIgdbId!.Value)
            .Distinct()
            .ToList();

        if (!parentIgdbIds.Any()) return;

        var parentGames = await GetOrCreateGamesAsync(parentIgdbIds, visited);

        foreach (var g in newGames.Where(g => g.ParentGameIgdbId.HasValue))
        {
            var parent = parentGames.FirstOrDefault(p => p.IgdbId == g.ParentGameIgdbId.Value);
            if (parent != null)
            {
                g.ParentGame = parent;
            }
        }

        await _context.SaveChangesAsync();
    }
}