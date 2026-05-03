using backend.Data;
using backend.Models.Entities;
using GameNewsHub.Api.External;
using GameNewsHub.Api.Services.Games;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace GameNewsHub.Api.Services.Events;

public class EventSyncService : IEventSyncService
{
    private readonly IIgdbClient _client;
    private readonly AppDbContext _context;
    private readonly ILogger<EventSyncService> _logger;
    private readonly IGameSyncService _gameSyncService;

    public EventSyncService(IIgdbClient client, AppDbContext context, ILogger<EventSyncService> logger, IGameSyncService gameSyncService)
    {
        _client = client;
        _context = context;
        _logger = logger;
        _gameSyncService = gameSyncService;
    }

    public async Task DiscoverNewEventsAsync()
    {
        /* Zrób zapytanie do IgdbClient o eventy +-30dni
         * Przyjmij te eventy w jakimś dto
         * sprawdź czy nie ma ich już w bazie
         * jeśli nie ma, to dodaj i ustaw status na Pending 
         */

        var from = DateTimeOffset.UtcNow.AddDays(-5).ToUnixTimeSeconds();
        var to = DateTimeOffset.UtcNow.AddDays(5).ToUnixTimeSeconds();
        var externalEvents = await _client.GetEventsFromIgdb(from, to);

        var existingItd = await _context.Events
            .Select(e => e.IgdbId)
            .ToListAsync();

        var newEvents = externalEvents
            .Where(e => !existingItd.Contains(e.Id))
            .Select(e => new Event
            {
                IgdbId = e.Id,
                Name = e.Name,
                StartTime = e.StartTime,
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

        var statusToProcess = new[] { EventSyncStatus.Pending, EventSyncStatus.NoData };

        // jak nie ma przez 3 dni od zakończenia, to pewnie nie będzie
        var timeThreshold = DateTimeOffset.UtcNow.AddDays(-3).ToUnixTimeSeconds();
        var noDataEvents = await _context.Events
            .Where(e => statusToProcess.Contains(e.Status))
            .Where(e => e.EndTime > timeThreshold)
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

            if (apiData.Games != null && apiData.Games.Any())
            {
                _logger.LogInformation($"Changing status of event: {e.Id} | {e.IgdbId} to Ready");
                
                // pobranie brakujących gier
                var gameIds = apiData.Games.Select(g => g.Id).ToList();
                var gamesInDb = await _gameSyncService.GetOrCreateGamesAsync(gameIds);
                
                // przypisanie gier do eventu
                e.Games = gamesInDb;

                e.Status = EventSyncStatus.Ready;
                // obliczenie score patrząc na genres
                
                
                
            }
            else
            {
                e.Status = EventSyncStatus.NoData;
            }
        }
    }
}