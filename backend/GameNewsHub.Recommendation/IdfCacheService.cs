using GameNewsHub.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace GameNewsHub.Recommendation;

/// <summary>
/// Trzyma osobne słowniki mapujące id cech na wyliczoną wagę rzadkości
/// (gdzie 1.0 jest powszechne, a wyższe oznaczają unikalne cechy)
/// </summary>
public class IdfCacheService
{
    private readonly IMemoryCache _cache;
    private readonly AppDbContext _context;
    private readonly ILogger<IdfCacheService> _logger;

    private const string GenreIdfKey = "IDF_GENRES";
    private const string PlatformIdfKey = "IDF_PLATFORMS";
    public IdfCacheService(
        IMemoryCache cache,
        AppDbContext context,
        ILogger<IdfCacheService> logger
    ) {
        _cache = cache;
        _context = context;
        _logger = logger;
    }

    public float GetGenreIdf(int genreId)
    {
        return GetFromDictionary(GenreIdfKey, genreId);
    }

    public float GetPlatformIdf(int platformId)
    {
        return GetFromDictionary(PlatformIdfKey, platformId);
    }

    private float GetFromDictionary(string cacheKey, int id)
    {
        if (_cache.TryGetValue(cacheKey, out Dictionary<int, float>? idfDict) && idfDict != null)
        {
            return idfDict.TryGetValue(id, out var idf) ? idf : 1.0f;
        }

        return 1.0f;
    }

    public async Task RefreshCacheAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("Refreshing IDF cache");

        var totalGamesCount = await _context.Games.AsNoTracking().CountAsync(ct);
        var totalEventsCount = await _context.Events.AsNoTracking().CountAsync(ct);

        if (totalGamesCount == 0 || totalEventsCount == 0)
        {
            _logger.LogWarning("Lacking db data");
            return;
        }

        var genreCounts = await _context.Games
            .AsNoTracking()
            .SelectMany(g => g.Genres)
            .GroupBy(g => g.Id)
            .Select(group => new { Id = group.Key, Count = group.Count() })
            .ToDictionaryAsync(x => x.Id, x => x.Count, ct);

        var platformCounts = await _context.Games
            .AsNoTracking()
            .SelectMany(g => g.Platforms.Select(p => new { GameId = g.Id, p.PlatformGroupId }))
            .Distinct()
            .GroupBy(x => x.PlatformGroupId)
            .Select(group => new { Id = group.Key, Count = group.Count() })
            .ToDictionaryAsync(x => x.Id, x => x.Count, ct);

        var genreIdfs = CalculateIdfDictionary(totalGamesCount, genreCounts);
        var platformIdfs = CalculateIdfDictionary(totalGamesCount, platformCounts);

        var cacheOptions = new MemoryCacheEntryOptions
        {
            Priority = CacheItemPriority.NeverRemove
        };

        _cache.Set(GenreIdfKey, genreIdfs, cacheOptions);
        _cache.Set(PlatformIdfKey, platformIdfs, cacheOptions);
        
        _logger.LogInformation("Idf cache refreshed");
    }

    private static Dictionary<int, float> CalculateIdfDictionary(int totalgames, Dictionary<int, int> featureCounts)
    {
        var result = new Dictionary<int, float>();

        foreach (var (featureId, count) in featureCounts)
        {
            var idf = (float)(Math.Log((double)(totalgames + 1) / (count + 1)) + 1.0f);
            result[featureId] = idf;
        }

        return result;
    }

}