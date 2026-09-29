using GameNewsHub.Data;
using GameNewsHub.Data.Entities;
using GameNewsHub.Sync.Dtos;
using GameNewsHub.Sync.External;
using GameNewsHub.Sync.Sync.Games;
using Microsoft.EntityFrameworkCore;

namespace GameNewsHub.Sync.Sync.Events;

public class EventSyncService : IEventSyncService
{
    private readonly IEventWeightCalculator _calculator;
    private readonly IIgdbClient _client;
    private readonly AppDbContext _context;
    private readonly IGameSyncService _gameSyncService;
    private readonly ILogger<EventSyncService> _logger;
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
        _syncDays = config.GetValue("SyncDaysRange", 30);
    }

    public async Task DiscoverNewEventsAsync()
    {
        var from = DateTimeOffset.UtcNow.AddDays(-_syncDays).ToUnixTimeSeconds();
        var to = DateTimeOffset.UtcNow.AddDays(10).ToUnixTimeSeconds();
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

        var newEvents = newExternalEvents.Select(e =>
        {
            var evt = new Event
            {
                IgdbId = e.IgdbId,
                Name = e.Name,
                StartTime = DateTimeOffset.FromUnixTimeSeconds(e.StartTime),
                EndTime = e.EndTime.HasValue ? DateTimeOffset.FromUnixTimeSeconds(e.EndTime.Value) : null,
                Description = e.Description,
                Status = EventSyncStatus.Pending,
                LiveStreamUrl = e.LiveStreamUrl ?? string.Empty,
            };
            var normalized = EventNameNormalizer.Normalize(evt.Name);
            evt.NormalizedName = string.IsNullOrWhiteSpace(normalized) ? evt.Name : normalized;
            return evt;
        }).ToList();

        var names = newEvents.Select(e => e.NormalizedName).Distinct().ToArray();

        var resolution = await ResolveSeriesAsync(names);

        var newSeriesCache = new Dictionary<string, EventSeries>();
        foreach (var evt in newEvents)
        {
            if (resolution.ExistingSeriesIds.TryGetValue(evt.NormalizedName, out var id))
            {
                evt.EventSeriesId = id;
            }
            else
            {
                var key = resolution.NewSeriesKeys[evt.NormalizedName];
                if (!newSeriesCache.TryGetValue(key, out var series))
                    newSeriesCache[key] = series = new EventSeries();
                evt.Series = series;
            }
        }

        _context.Events.AddRange(newEvents);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Added {newEvents.Count} new events");
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

    public async Task HydrateEventsAsync()
    {
        /*
         * szuka w bazie zakończone eventy, ze statusem NoData
         * wysyła do clienta, który pobiera dane
         * jeśli dany event ma dane, to aktualizuje bazę i status na Ready?
         *
         */

        // jak nie ma przez 3 dni od zakończenia, to pewnie nie będzie
        var timeThreshold = DateTimeOffset.UtcNow.AddDays(-_syncDays);
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
            e.EndTime = apiData.EndTime.HasValue //na wypadek żeby jeszcze nie było
                ? DateTimeOffset.FromUnixTimeSeconds(apiData.EndTime.Value)
                : null;
            e.LiveStreamUrl = apiData.LiveStreamUrl ?? string.Empty;

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
                foreach (var weight in calculatedWeights) e.GenreWeights.Add(weight);

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

public sealed record FuzzyRow(string NormalizedName, int SeriesId);
public sealed record PairRow(string NameA, string NameB, float Score);