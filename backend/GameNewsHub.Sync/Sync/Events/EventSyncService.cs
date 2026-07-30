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

    public EventSyncService(IIgdbClient client,
        AppDbContext context,
        ILogger<EventSyncService> logger,
        IGameSyncService gameSyncService,
        IEventWeightCalculator calculator)
    {
        _client = client;
        _context = context;
        _logger = logger;
        _gameSyncService = gameSyncService;
        _calculator = calculator;
    }

    public async Task DiscoverNewEventsAsync()
    {
        /* Zrób zapytanie do IgdbClient o eventy +-30dni
         * Przyjmij te eventy w jakimś dto
         * sprawdź czy nie ma ich już w bazie
         * jeśli nie ma, to dodaj i ustaw status na Pending 
         */

        var from = DateTimeOffset.UtcNow.AddDays(-10).ToUnixTimeSeconds();
        var to = DateTimeOffset.UtcNow.AddDays(10).ToUnixTimeSeconds();
        var externalEvents = await _client.GetEventsFromIgdb(from, to);

        var incomingIds = externalEvents.Select(e => e.Id).ToList();

        var existingItd = await _context.Events
            .Where(e => incomingIds.Contains(e.IgdbId))
            .Select(e => e.IgdbId)
            .ToListAsync();

        var newEvents = externalEvents
            .Where(e => !existingItd.Contains(e.Id))
            .Select(e => new Event
            {
                IgdbId = e.Id,
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
        var timeThreshold = DateTimeOffset.UtcNow.AddDays(-10).ToUnixTimeSeconds();
        var noDataEvents = await _context.Events
            .Where(e => e.Status != EventSyncStatus.Ready)
            .Where(e => e.StartTime > timeThreshold)
            .Select(e => e.IgdbId)
            .ToListAsync();

        if (!noDataEvents.Any())
        {
            return;
        }
        
        var res = await _client.UpdateEventsFromIgdb(noDataEvents);

        var igdbIds = res.Select(e => e.Id).ToList();

        var events = await _context.Events
            .Where(e => igdbIds.Contains(e.IgdbId))
            .ToListAsync();

        foreach (var e in events)
        {
            var apiData = res.FirstOrDefault(r => r.Id == e.IgdbId);
            if (apiData == null) continue;

            e.Name = apiData.Name;
            e.Description = apiData.Description;
            e.EndTime = apiData.EndTime; //na wypadek żeby jeszcze nie było

            if (apiData.Games != null && apiData.Games.Any())
            {
                _logger.LogInformation($"Changing status of event: {e.Id} | {e.IgdbId} to Ready");
                
                // pobranie brakujących gier
                var gameIds = apiData.Games.Select(g => g.Id).ToList();
                var gamesInDb = await _gameSyncService.GetOrCreateGamesAsync(gameIds);
                
                // przypisanie gier do eventu
                e.Games = gamesInDb;

                // obliczenie score patrząc na genres
                
                if (e.GenreWeights != null) e.GenreWeights.Clear();

                var calculatedWeights = _calculator.CalculateScores(e.Id, gamesInDb);

                foreach (var weight in calculatedWeights)
                {
                    e.GenreWeights.Add(weight);
                }

                e.Status = EventSyncStatus.Ready;
            }
            else
            {
                e.Status = EventSyncStatus.NoData;
            }
        }

        await _context.SaveChangesAsync();
    }
}