using GameNewsHub.Data;
using GameNewsHub.Data.Entities;
using GameNewsHub.External.Igdb.Client;
using GameNewsHub.External.Igdb.Dtos;
using GameNewsHub.Sync.Core.Events.Calculators;
using GameNewsHub.Sync.Core.Events.Normalizers;
using GameNewsHub.Sync.Core.Games;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GameNewsHub.Sync.Core.Events;

public class EventDiscoveryService
{
    private readonly IEventWeightCalculator _calculator;
    private readonly IIgdbClient _client;
    private readonly AppDbContext _context;
    private readonly IGameSyncService _gameSyncService;
    private readonly ILogger<EventDiscoveryService> _logger;
    private readonly int _syncDays;

    public EventDiscoveryService(IIgdbClient client,
        AppDbContext context,
        ILogger<EventDiscoveryService> logger,
        IGameSyncService gameSyncService,
        IEventWeightCalculator calculator
        )
    {
        _client = client;
        _context = context;
        _logger = logger;
        _gameSyncService = gameSyncService;
        _calculator = calculator;
    }

    public async Task DiscoverNewEventsAsync(long from, long to)
    {
        var externalEvents = await _client.GetEventsFromIgdb(from, to);

        var incomingIds = externalEvents.Select(e => e.IgdbId).ToList();
        var existingItd = (await _context.Events
            .Where(e => incomingIds.Contains(e.IgdbId))
            .Select(e => e.IgdbId)
            .ToListAsync()).ToHashSet();

        var newExternalEvents = externalEvents
            .DistinctBy(e => e.IgdbId)
            .Where(e => !existingItd.Contains(e.IgdbId))
            .ToList();
        if (!newExternalEvents.Any()) return;

        var newEvents = newExternalEvents.Select(MapToEntity).ToList();

        var names = newEvents.Select(e => e.NormalizedName).Distinct().ToArray();
        var resolution = await ResolveSeriesAsync(names);

        AssignEventSeries(newEvents, resolution);
        
        _context.Events.AddRange(newEvents);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation($"Added {newEvents.Count} new events");
    }

    private Event MapToEntity(IgdbDiscoverEventDto dto)
    {
        var name = dto.Name.Trim();
        var normalized = EventNameNormalizer.Normalize(name);
        var normalizedFinal = string.IsNullOrWhiteSpace(normalized) ? dto.Name : normalized;
        
        return new Event
        {
            IgdbId = dto.IgdbId,
            Name = dto.Name,
            NormalizedName = normalizedFinal,
            StartTime = DateTimeOffset.FromUnixTimeSeconds(dto.StartTime),
            EndTime = dto.EndTime.HasValue ? DateTimeOffset.FromUnixTimeSeconds(dto.EndTime.Value) : null,
            Description = dto.Description ?? string.Empty,
            LiveStreamUrl = dto.LiveStreamUrl ?? string.Empty,
            Status = EventSyncStatus.Pending,
        };
        
    }
    private void AssignEventSeries(List<Event> events, SeriesResolution resolution)
    {
        var newSeriesCache = new Dictionary<string, EventSeries>();

        foreach (var evt in events)
        {
            if (resolution.ExistingSeriesIds.TryGetValue(evt.NormalizedName, out var id))
            {
                evt.EventSeriesId = id;
            }
            else
            {
                var key = resolution.NewSeriesKeys[evt.NormalizedName];
                if (!newSeriesCache.TryGetValue(key, out var series))
                {
                    newSeriesCache[key] = series = new EventSeries();
                }
                evt.Series = series;
            }
        }
    }

    private sealed record SeriesResolution(
        Dictionary<string, int> ExistingSeriesIds, //nazwa -> id istniejącej serii
        Dictionary<string, string> NewSeriesKeys); //nazwa -> klucz nowej serii (reprezentant grupy)

    private async Task<SeriesResolution> ResolveSeriesAsync(string[] names)
    {
        var existing = await FindExistingSeriesAsync(names);
        var unmatched = names.Where(n => !existing.ContainsKey(n)).ToArray();
        var newKeys = await GroupNewNamesAsync(unmatched);

        return new SeriesResolution(existing, newKeys);
    }

    private async Task<Dictionary<string, int>> FindExistingSeriesAsync(string[] names)
    {
        var result = (await _context.Events
                .Where(e => names.Contains(e.NormalizedName))
                .Select(e => new { e.NormalizedName, SeriesId = e.EventSeriesId })
                .Distinct()
                .ToListAsync())
            .GroupBy(x => x.NormalizedName)
            .ToDictionary(g => g.Key, g => g.First().SeriesId);

        var unmatched = names.Where(n => !result.ContainsKey(n)).ToArray();
        if (unmatched.Length == 0) return result;


        var fuzzy = await _context.Database.SqlQuery<FuzzyRow>
        ($"""
          SELECT n.name AS "NormalizedName", m."EventSeriesId" AS "SeriesId"
          FROM unnest({unmatched}::text[]) AS n(name)
          CROSS JOIN LATERAL (
              SELECT e."EventSeriesId"
              FROM "Events" e
              WHERE e."NormalizedName" % n.name
              AND similarity(e."NormalizedName", n.name) >= 0.85
              ORDER BY similarity(e."NormalizedName", n.name) DESC
          LIMIT 1
          ) m 
          """).ToListAsync();

        foreach (var r in fuzzy)
            result[r.NormalizedName] = r.SeriesId;

        return result;
    }

    private async Task<Dictionary<string, string>> GroupNewNamesAsync(string[] names)
    {
        var representative = new Dictionary<string, string>();
        if (names.Length == 0) return representative;

        var pairs = names.Length < 2
            ? new List<PairRow>()
            : await _context.Database.SqlQuery<PairRow>
            ($"""
              SELECT a.name AS "NameA", b.name AS "NameB", similarity(a.name, b.name) AS "Score"
              FROM unnest({names}::text[]) AS a(name)
              JOIN unnest({names}::text[]) AS b(name) ON a.name < b.name
              WHERE similarity(a.name, b.name) >= 0.85
              """).ToListAsync();

        var neighbors = pairs
            .SelectMany(p => new[]
            {
                (Name: p.NameA, Other: p.NameB, p.Score),
                (Name: p.NameB, Other: p.NameA, p.Score)
            })
            .ToLookup(x => x.Name, x => (x.Other, x.Score));

        foreach (var name in names.OrderBy(n => n, StringComparer.Ordinal)) // deterministyczna kolejność
        {
            var best = neighbors[name]
                .Where(n => representative.TryGetValue(n.Other, out var rep) && rep == n.Other)
                .OrderByDescending(n => n.Score)
                .Select(n => n.Other)
                .FirstOrDefault();

            representative[name] = best ?? name;
        }

        return representative;
    }

}

public sealed record FuzzyRow(string NormalizedName, int SeriesId);
public sealed record PairRow(string NameA, string NameB, float Score);