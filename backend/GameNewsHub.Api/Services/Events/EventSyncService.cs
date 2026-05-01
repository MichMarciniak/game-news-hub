using backend.Data;
using backend.Models.Entities;
using GameNewsHub.Api.External;
using Microsoft.EntityFrameworkCore;

namespace GameNewsHub.Api.Services.Events;

public class EventSyncService : IEventSyncService
{
    private readonly IIgdbClient _client;
    private readonly AppDbContext _context;
    private readonly ILogger<EventSyncService> _logger;

    public EventSyncService(IIgdbClient client, AppDbContext context, ILogger<EventSyncService> logger)
    {
        _client = client;
        _context = context;
        _logger = logger;
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
         * pomyśl co jeśli api nie da endTime
         */

        var statusToProcess = new[] { EventSyncStatus.Pending, EventSyncStatus.NoData };

        var timeNow = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var noDataEvents = await _context.Events
            .Where(e => statusToProcess.Contains(e.Status))
            .Where(e => e.EndTime > timeNow)
            .Select(e => e.Id)
            .ToListAsync();

        if (noDataEvents.Any())
        {
            var req = await _client.UpdateEventsFromIgdb(noDataEvents);
        }


    }
}