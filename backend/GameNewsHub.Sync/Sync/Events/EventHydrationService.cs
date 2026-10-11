using GameNewsHub.Data;
using GameNewsHub.Data.Entities;
using GameNewsHub.External.Igdb.Client;
using GameNewsHub.External.Igdb.Dtos;
using GameNewsHub.Sync.Sync.Games;
using Microsoft.EntityFrameworkCore;

namespace GameNewsHub.Sync.Sync.Events;

public class EventHydrationService
{
    
    private readonly IEventWeightCalculator _calculator;
    private readonly IIgdbClient _client;
    private readonly AppDbContext _context;
    private readonly IGameSyncService _gameSyncService;
    private readonly ILogger<EventHydrationService> _logger;
    private readonly int _syncDays;

    public EventHydrationService(IIgdbClient client,
        AppDbContext context,
        ILogger<EventHydrationService> logger,
        IGameSyncService gameSyncService,
        IEventWeightCalculator calculator,
        IConfiguration config)
    {
        _client = client;
        _context = context;
        _logger = logger;
        _gameSyncService = gameSyncService;
        _calculator = calculator;
        _syncDays = config.GetValue("SyncDaysRange", 30);
    }
    
    public async Task HydrateEventsAsync()
    {
        /*
         * szuka w bazie zakończone eventy, ze statusem NoData
         * wysyła do clienta, który pobiera dane
         * jeśli dany event ma dane, to aktualizuje bazę i status na Ready?
         *
         */

        var igdbIds = await GetEventIdsAsync();
        if (!igdbIds.Any()) return;

        var apiEvents = await _client.UpdateEventsFromIgdb(igdbIds);
        if (!apiEvents.Any()) return;

        var apiById = apiEvents.ToDictionary(r => r.IgdbId);
        var gamesMap = await LoadGamesAsync(apiEvents);

        var events = await _context.Events
            .Include(e => e.GenreWeights)
            .Where(e => igdbIds.Contains(e.IgdbId))
            .ToListAsync();

        foreach (var evt in events)
        {
            ApplyApiData(evt, apiById[evt.IgdbId], gamesMap);
        }

        await _context.SaveChangesAsync();
    }

    private async Task<List<int>> GetEventIdsAsync()
    {
        var timeThreshold = DateTimeOffset.UtcNow.AddDays(-_syncDays);
        
        return await _context.Events
            .Where(e => e.Status != EventSyncStatus.Ready)
            .Where(e => e.StartTime > timeThreshold)
            .Select(e => e.IgdbId)
            .ToListAsync();
    }

    private async Task<Dictionary<int, Game>> LoadGamesAsync(IEnumerable<IgdbEventResponse> apiEvents)
    {

        var gameIds = apiEvents
            .Where(r => r.Games != null)
            .SelectMany(r => r.Games)
            .Select(g => g.IgdbId)
            .Distinct()
            .ToList();

        if (!gameIds.Any()) return new Dictionary<int, Game>();

        var games = await _gameSyncService.GetOrCreateGamesAsync(gameIds);
        return games.ToDictionary(g => g.IgdbId);
    }

    private void ApplyApiData(Event evt, IgdbEventResponse apiData, Dictionary<int, Game> gamesMap)
    {
        evt.Name = apiData.Name;
        evt.Description = apiData.Description;
        evt.StartTime = DateTimeOffset.FromUnixTimeSeconds(apiData.StartTime);
        evt.EndTime = apiData.EndTime.HasValue
            ? DateTimeOffset.FromUnixTimeSeconds(apiData.EndTime.Value)
            : null;
        evt.LiveStreamUrl = apiData.LiveStreamUrl ?? string.Empty;

        var games = ResolveGames(apiData, gamesMap);
        if (!games.Any())
        {
            SetStatus(evt, EventSyncStatus.NoData);
            return;
        }

        evt.Games = games;
        ReplaceGenreWeights(evt, games);
        SetStatus(evt, EventSyncStatus.Ready);
    }

    private static List<Game> ResolveGames(IgdbEventResponse apiData, Dictionary<int, Game> gamesMap)
    {
        return apiData.Games?
                .Select(g => gamesMap.GetValueOrDefault(g.IgdbId))
                .OfType<Game>() //filtruje nulle jakby były
                .ToList()
            ?? new List<Game>();
    }

    private void ReplaceGenreWeights(Event evt, List<Game> games)
    {
        evt.GenreWeights.Clear();
        foreach (var weight in _calculator.CalculateScores(evt.Id, games))
        {
            evt.GenreWeights.Add(weight);
        }
    }

    private void SetStatus(Event evt, EventSyncStatus status)
    {
        _logger.LogInformation("Changing status of event: {Id} | {IgdbId} to {Status}",
            evt.Id, evt.IgdbId, status);
        evt.Status = status;
    }
}