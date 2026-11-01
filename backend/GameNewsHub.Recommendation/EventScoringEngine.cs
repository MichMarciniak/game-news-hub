using GameNewsHub.Data;
using GameNewsHub.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameNewsHub.Recommendation;

public static class LabelCodes
{
    public const string FollowedGame = "FollowedGame";
    public const string FollowedGameAddon = "FollowedGameAddon";
    public const string FollowedSeries = "FollowedSeries";
}


public class EventScoringEngine
{
    private readonly AppDbContext _context;
    private readonly IdfCacheService _cache;

    private const float MomentumSmoothingK = 50.0f;
    private const float MomentumCap = 0.5f; //max. 50% wzrostu w 24h

    private const float GameSaturation = 1.0f; //raw / (raw + c)
    private const float ParentGameFactor = 0.5f;
    private const float MaxAlpha = 0.9f;

    private const float BetaMomentumUpcoming = 0.25f;
    private const float GameWeightUpcoming = 0.30f;
    private const float GenreWeightUpcoming = 0.35f;
    private const float PlatformWeightUpcoming = 0.10f;
    private const float SeriesWeightUpcoming = 0.25f;

    private const float GameWeightArchived = 0.65f;
    private const float GenreWeightArchived = 0.25f;
    private const float PlatformWeightArchived = 0.05f;
    private const float SeriesWeightArchived = 0.05f;

    public EventScoringEngine(AppDbContext context, IdfCacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<List<ScoredEventResult>> ScoreEventsAsync(UserProfileVector userProfile,
        IEnumerable<EventVector> events,
        bool isUpcomingSection,
        int maxFollowers,
        CancellationToken ct = default)
    {
        var eventList = events.ToList();
        if (eventList.Count == 0) return [];

        var baseline = isUpcomingSection
            ? await LoadFollowerBaselineAsync(eventList.Select(e => e.EventId).ToList(), ct)
            : new Dictionary<int, int>();

        return ScoreEvents(userProfile, eventList, baseline, isUpcomingSection, maxFollowers);
    }


    /// <summary>
    /// Czysta funkcja, bez dostępu do bazy
    /// </summary>
    public List<ScoredEventResult> ScoreEvents(
        UserProfileVector profile,
        IReadOnlyList<EventVector> events,
        IReadOnlyDictionary<int, int> followerBaseline,
        bool isUpcomingSection, int maxFollowers)
    {
        if (events.Count == 0) return [];

        float wGame = isUpcomingSection ? GameWeightUpcoming : GameWeightArchived;
        float wGenre = isUpcomingSection ? GenreWeightUpcoming : GenreWeightArchived;
        float wPlatform = isUpcomingSection ? PlatformWeightUpcoming : PlatformWeightArchived;
        float wSeries = isUpcomingSection ? SeriesWeightUpcoming : SeriesWeightArchived;
        
        float beta = isUpcomingSection ? BetaMomentumUpcoming : 0.0f;
        float alpha = Math.Min(MaxAlpha, profile.Alpha);
        
        float logMax = MathF.Log(Math.Max(1, maxFollowers) + 1);

        float genreNorm = Norm(profile.GenreWeights, _cache.GetGenreIdf);
        float platformNorm = Norm(profile.PlatformGroupWeights, _cache.GetPlatformIdf);

        var results = new List<ScoredEventResult>(events.Count);

        foreach (var evt in events)
        {
            var result = new ScoredEventResult { EventVector = evt };

            float gameRaw = 0f;
            int directHits = 0, parentHits = 0;

            foreach (var gameId in evt.DirectGameIds)
            {
                if (profile.GameWeights.TryGetValue(gameId, out var gw))
                {
                    gameRaw += gw;
                    directHits++;
                }
            }

            foreach (var parentId in evt.ParentGameIds)
            {
                if (profile.GameWeights.TryGetValue(parentId, out var pw))
                {
                    gameRaw += pw * ParentGameFactor;
                    parentHits++;
                }
            }

            float gameFacet = gameRaw / (gameRaw + GameSaturation); // 0 ... 1
            
            if (directHits > 0) result.Labels.Add(new RecLabel(LabelCodes.FollowedGame, directHits));
            if (parentHits > 0) result.Labels.Add(new RecLabel(LabelCodes.FollowedGameAddon, parentHits));

            float genreFacet = Cosine(profile.GenreWeights, genreNorm, evt.GenreWeights, _cache.GetGenreIdf);
            float platformFacet = Cosine(profile.PlatformGroupWeights, platformNorm, evt.PlatformGroupWeights,
                _cache.GetPlatformIdf);

            float seriesFacet = 0f;
            if (profile.SeriesWeights.TryGetValue(evt.SeriesId, out var sw))
            {
                seriesFacet = Math.Min(1f, sw);
                result.Labels.Add(new RecLabel(LabelCodes.FollowedSeries, 1));
            }

            result.GameFacetContribution = gameFacet * wGame;
            result.GenreFacetContribution = genreFacet * wGenre;
            result.PlatformGroupFacetContribution = platformFacet * wPlatform;
            result.SeriesFacetContribution = seriesFacet * wSeries;

            result.PersonalScore = result.GameFacetContribution
                                   + result.GenreFacetContribution
                                   + result.PlatformGroupFacetContribution
                                   + result.SeriesFacetContribution;

            result.PopularityScore = Math.Min(1f, MathF.Log(evt.FollowerCount + 1) / logMax);

            if (isUpcomingSection)
            {
                int past = followerBaseline.TryGetValue(evt.EventId, out var p) ? p : evt.FollowerCount;
                float raw = Math.Max(0, evt.FollowerCount - past) / (past + MomentumSmoothingK);
                result.MomentumScore = Math.Min(1f, raw / MomentumCap);
            }

            result.FinalScore = (alpha * result.PersonalScore)
                                + ((1f - alpha) * result.PopularityScore)
                                + (beta * result.MomentumScore);
            
            results.Add(result);
        }

        return results.OrderByDescending(r => r.FinalScore).ToList();
    }
    
    /// <summary>
    /// Snapshot najbliższy 24h wstecz; jak nie ma to najstarszy
    /// </summary>
    private async Task<Dictionary<int, int>> LoadFollowerBaselineAsync(List<int> eventIds, CancellationToken ct)
    {
        var target = DateTimeOffset.UtcNow.AddHours(-24);

        var rows = await _context.EventFollowerSnapshots
            .AsNoTracking()
            .Where(s => eventIds.Contains(s.EventId) && s.RecordedAt >= target.AddHours(-6))
            .Select(s => new { s.EventId, s.RecordedAt, s.FollowerCount })
            .ToListAsync(ct);

        return rows.GroupBy(r => r.EventId).ToDictionary(
            g => g.Key,
            g => g.OrderBy(r => Math.Abs((r.RecordedAt - target).Ticks)).First().FollowerCount);
    }

    private static float Norm(Dictionary<int, float> vector, Func<int, float> idf)
    {
        float sum = 0f;
        foreach (var (id, w) in vector)
        {
            float x = w * idf(id);
            sum += x * x;
        }

        return MathF.Sqrt(sum);
    }

    private static float Cosine(
        Dictionary<int, float> user, float userNorm,
        Dictionary<int, float> evt, Func<int, float> idf)
    {
        if (userNorm == 0f) return 0f;

        float dot = 0f, evtSq = 0f;
        foreach (var (id, ew) in evt)
        {
            float i = idf(id);
            float ex = ew * i;
            evtSq += ex * ex;
            if (user.TryGetValue(id, out var uw)) dot += uw * i * ex;
        }

        return evtSq == 0f ? 0f : dot / (userNorm * MathF.Sqrt(evtSq));
    }
}