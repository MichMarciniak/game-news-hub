using backend.Data;
using GameNewsHub.Data.Entities;
using GameNewsHub.Sync.Dtos;
using GameNewsHub.Sync.External;
using Microsoft.EntityFrameworkCore;

namespace GameNewsHub.Sync.Sync.Games;

public class GameSyncService : IGameSyncService
{
    private readonly IIgdbClient _client;
    private readonly IGenreSyncService _genreService;
    private readonly AppDbContext _context;
    private readonly ILogger<GameSyncService> _logger;

    public GameSyncService(IIgdbClient client, IGenreSyncService service, AppDbContext context, ILogger<GameSyncService> logger)
    {
        _client = client;
        _genreService = service;
        _context = context;
        _logger = logger;
    }

    public async Task SyncUpcomingGamesAsync(int limit = 10)
    {
        var rawGames = await _client.GetGamesFromIgdb(limit);

        foreach (var dto in rawGames)
        {
            if (await _context.Games.AnyAsync(g => g.IgdbId == dto.Id)) continue;

            var game = new Game
            {
                IgdbId = dto.Id,
                Title = dto.Name,
                Summary = dto.Summary,
                CoverUrl = dto.Cover?.Url?.Replace("t_thumb", "t_720p").Insert(0, "https:")
            };

            if (dto.Genres != null)
            {
                foreach (var gDto in dto.Genres)
                {
                    var genre = await _genreService.GetOrCreateAsync(gDto.Id, gDto.Name);
                    game.Genres.Add(genre);
                }
            }

            _context.Games.Add(game);
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("Game sync finished.");
    }

    public async Task<ICollection<Game>> GetOrCreateGamesAsync(IEnumerable<int> gameIds)
    {
        return await GetOrCreateGamesAsync(gameIds, new HashSet<int>());
    }

    // przeciążenie z visited, chroni przed nieskonczona rekurencja
    // gdyby parent_game tworzył cykl w igdb
    public async Task<ICollection<Game>> GetOrCreateGamesAsync(IEnumerable<int> gameIds, HashSet<int> visited)
    {
        if (gameIds == null || !gameIds.Any()) return new List<Game>();

        var idsToProcess = gameIds.Where(id => visited.Add(id)).ToList();
        if (!idsToProcess.Any()) return new List<Game>();
        
        var existingGames = await _context.Games
            .Include(g => g.Genres)
            .Where(g => idsToProcess.Contains(g.IgdbId))
            .ToListAsync();

        var existingIds = existingGames.Select(g => g.IgdbId).ToList();
        var missingIds = idsToProcess.Except(existingIds).ToList();

        if (!missingIds.Any())
        {
            return existingGames;
        }
        
        // pobierz brakujące dane z igdb api
        var externalGames = await _client.UpdateMissingGames(missingIds);

        var allGenres = externalGames
            .SelectMany(g => g.Genres ?? Enumerable.Empty<IgdbGenreResponse>())
            .Distinct()
            .ToList();
        var genres = await _genreService.GetOrCreateBatchAsync(allGenres);

        var newGames = externalGames.Select(dto =>
        {
            string rawUrl = dto.Cover?.Url;
            string formattedCoverUrl = null;
            if (!string.IsNullOrEmpty(rawUrl))
            {
                var bigCoverUrl = rawUrl.Replace("t_thumb", "t_cover_big");
                formattedCoverUrl = bigCoverUrl.StartsWith("//")
                    ? $"https:{bigCoverUrl}"
                    : bigCoverUrl;
            }
            
            return new Game
            {
                IgdbId = dto.Id,
                Title = dto.Name,
                Summary = dto.Summary,
                CoverUrl = formattedCoverUrl,
                Type = dto.GameType != null ? (GameType)dto.GameType.Id : GameType.MainGame,
                ParentGameIgdbId = dto.ParentGame?.Id,
                Genres = MapGenres(dto.Genres, genres)
            };
        }).ToList();
        
        _context.Games.AddRange(newGames);
        await _context.SaveChangesAsync();

        var allGames = existingGames.Concat(newGames).ToList();

        await PullParentGames(newGames, visited);

        return allGames;
    }

    private ICollection<Genre> MapGenres(IEnumerable<IgdbGenreResponse> dtos, IEnumerable<Genre> genres)
    {
        if (dtos == null || !dtos.Any()) return new List<Genre>();
        
        var ids = dtos.Select(d => d.Id).ToList();
        return genres.Where(g => ids.Contains(g.IgdbId)).ToList();
    }

    private async Task PullParentGames(List<Game> games, HashSet<int> visited)
    {
        var parentIgdbIds = games
            .Where(g => g.ParentGameIgdbId.HasValue)
            .Select(g => g.ParentGameIgdbId!.Value)
            .Distinct()
            .ToList();

        if (!parentIgdbIds.Any()) return;

        await GetOrCreateGamesAsync(parentIgdbIds, visited);

        var parentGames = await _context.Games
            .Where(g => parentIgdbIds.Contains(g.IgdbId))
            .ToListAsync();

        foreach (var g in games.Where(g => g.ParentGameIgdbId.HasValue))
        {
            var parent = parentGames.FirstOrDefault(p => p.IgdbId == g.ParentGameIgdbId.Value);
            if (parent != null)
            {
                g.ParentGameId = parent.Id;
            }
        }

        await _context.SaveChangesAsync();
    }

}