using backend.Data;
using GameNewsHub.Data.Entities;
using GameNewsHub.Sync.Sync.Games;
using GameNewsHub.Sync.External;
using Microsoft.EntityFrameworkCore;

namespace GameNewsHub.Sync.Sync.Events;

public class EventSyncService : IEventSyncService
{
    private readonly IIgdbClient _client;
    private readonly AppDbContext _context;
    private readonly ILogger<EventSyncService> _logger;
    private readonly IGameSyncService _gameSyncService;
    private readonly IEventWeightCalculator _calculator;
    private readonly int _syncDays;

    public EventSyncService(IIgdbClient client,
        AppDbContext context,
        ILogger<EventSyncService> logger,
        IGameSyncService gameSyncService,
        IEventWeightCalculator calculator,
        IConfiguration config)
    {
        _client = client;
        _context = context;
        _logger = logger;
        _gameSyncService = gameSyncService;
        _calculator = calculator;
        _syncDays = config.GetValue<int>("SyncDaysRange", 30);
    }

    public async Task DiscoverNewEventsAsync()
    {
        /* Zrób zapytanie do IgdbClient o eventy +-30dni
         * Przyjmij te eventy w jakimś dto
         * sprawdź czy nie ma ich już w bazie
         * jeśli nie ma, to dodaj i ustaw status na Pending 
         */

        var from = DateTimeOffset.UtcNow.AddDays(-_syncDays).ToUnixTimeSeconds();
        var to = DateTimeOffset.UtcNow.AddDays(10).ToUnixTimeSeconds();
        var externalEvents = await _client.GetEventsFromIgdb(from, to);

        var incomingIds = externalEvents.Select(e => e.IgdbId).ToList();

        var existingItd = await _context.Events
            .Where(e => incomingIds.Contains(e.IgdbId))
            .Select(e => e.IgdbId)
            .ToListAsync();

        var newEvents = externalEvents
            .Where(e => !existingItd.Contains(e.IgdbId))
            .Select(e => new Event
            {
                IgdbId = e.IgdbId,
                Name = e.Name,
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                Description = e.Description,
                Status = EventSyncStatus.Pending
            });

        if (newEvents.Any())
        {
            _context.Events.AddRange(newEvents);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Added {newEvents.Count()} new events");
        }
    }

    public async Task HydrateEventsAsync()
    {
        /*
         * szuka w bazie zakończone eventy, ze statusem NoData
         * wysyła do clienta, który pobiera dane
         * jeśli dany event ma dane, to aktualizuje bazę i status na Ready?
         *
         */

        // jak nie ma przez 3 dni od zakończenia, to pewnie nie będzie
        var timeThreshold = DateTimeOffset.UtcNow.AddDays(-_syncDays).ToUnixTimeSeconds();
        var noDataEvents = await _context.Events
            .Where(e => e.Status != EventSyncStatus.Ready)
            .Where(e => e.StartTime > timeThreshold)
            .Select(e => e.IgdbId)
            .ToListAsync();

        if (!noDataEvents.Any()) return;
        
        var res = await _client.UpdateEventsFromIgdb(noDataEvents);
        var igdbIds = res.Select(e => e.IgdbId).ToList();

        var events = await _context.Events
            .Include(e => e.GenreWeights)
            .Where(e => igdbIds.Contains(e.IgdbId))
            .ToListAsync();

        // zbierz unikalne id gier ze wszystkich eventów
        var allGameIds = res
            .Where(r => r.Games != null)
            .SelectMany(r => r.Games)
            .Select(g => g.Id)
            .Distinct()
            .ToList();

        var gamesMap = new Dictionary<int, Game>();
        if (allGameIds.Any())
        {
            var fetchedGames = await _gameSyncService.GetOrCreateGamesAsync(allGameIds);
            gamesMap = fetchedGames.ToDictionary(g => g.IgdbId);
        }
        
        // przypisz dane w pętli (bez requestów)
        foreach (var e in events)
        {
            var apiData = res.FirstOrDefault(r => r.IgdbId == e.IgdbId);
            if (apiData == null) continue;

            e.Name = apiData.Name;
            e.Description = apiData.Description;
            e.EndTime = apiData.EndTime; //na wypadek żeby jeszcze nie było

            if (apiData.Games != null && apiData.Games.Any())
            {
                var eventGameIds = apiData.Games.Select(g => g.Id);
                var eventGames = eventGameIds
                    .Where(id => gamesMap.ContainsKey(id))
                    .Select(id => gamesMap[id])
                    .ToList();
                
                e.Games = eventGames;

                // aktualizacja wag
                e.GenreWeights.Clear();
                var calculatedWeights = _calculator.CalculateScores(e.Id, eventGames);
                foreach (var weight in calculatedWeights)
                {
                    e.GenreWeights.Add(weight);
                }

                _logger.LogInformation($"Changing status of event: {e.Id} | {e.IgdbId} to Ready");
                e.Status = EventSyncStatus.Ready;
            }
            else
            {
                _logger.LogInformation($"Changing status of event: {e.Id} | {e.IgdbId} to NoData");
                e.Status = EventSyncStatus.NoData;
            }
        }
        await _context.SaveChangesAsync();
    }
}