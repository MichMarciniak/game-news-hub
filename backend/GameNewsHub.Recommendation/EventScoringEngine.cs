using GameNewsHub.Data;
using GameNewsHub.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameNewsHub.Recommendation;

public class EventScoringEngine
{
    private readonly AppDbContext _context;
    private readonly IdfCacheService _cache;

    private const float MomentumSmoothingK = 50.0f;

    private const float BetaMomentumUpcoming = 0.25f;
    private const float GameWeightUpcoming = 0.35f;
    private const float GenreWeightUpcoming = 0.75f;
    private const float PlatformWeightUpcoming = 0.15f;
    private const float SeriesWeightUpcoming = 0.15f;

    private const float GameWeightArchived = 0.70f;
    private const float GenreWeightArchived = 0.20f;
    private const float PlatformWeightArchived = 0.05f;
    private const float SeriesWeightArchived = 0.05f;

    public EventScoringEngine(AppDbContext context, IdfCacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<List<ScoredEventResult>> ScoreEventsAsync(
        UserProfileVector userProfile,
        IEnumerable<EventVector> events,
        bool isUpcomingSection,
        CancellationToken ct = default)
    {
        var eventList = events.ToList();
        if (!eventList.Any()) return new List<ScoredEventResult>();

        var eventIds = eventList.Select(e => e.EventId).ToList();
        var snapshotCutoff = DateTimeOffset.UtcNow.AddHours(-24);

        var pastSnapshots = await _context.EventFollowerSnapshots
            .AsNoTracking()
            .Where(s => eventIds.Contains(s.EventId) && s.RecordedAt <= snapshotCutoff)
            .GroupBy(s => s.EventId)
            .Select(g => new
            {
                EventId = g.Key,
                FollowerCount24h = g.OrderByDescending(s => s.RecordedAt).First().FollowerCount
            })
            .ToDictionaryAsync(x => x.EventId, x => x.FollowerCount24h, ct);

        float maxFollowers = Math.Max(1.0f, eventList.Max(e => e.FollowerCount));

        var results = new List<ScoredEventResult>();

        foreach (var evt in eventList)
        {
            var result = new ScoredEventResult { EventVector = evt };

            float gameFacetRaw = 0.0f;
            foreach (var gameId in evt.DirectGameIds)
            {
                if (userProfile.GameWeights.TryGetValue(gameId, out var weight))
                {
                    gameFacetRaw += weight;
                    result.Labels.Add("Contains followed game");
                }
            }

            foreach (var pgi in evt.ParentGameIds)
            {
                if (userProfile.GameWeights.TryGetValue(pgi, out var parentWeight))
                {
                    gameFacetRaw += parentWeight * 0.5f; // relacja 
                    result.Labels.Add("Contains dlc to a your followed game");
                }
            }

            float genreFacetRaw = 0.0f;
            foreach (var (genreId, evGenreWeight) in evt.GenreWeights)
            {
                if (userProfile.GenreWeights.TryGetValue(genreId, out var weight))
                {
                    float idf = _cache.GetGenreIdf(genreId);
                    genreFacetRaw += weight * evGenreWeight * idf;
                }
            }

            float platformFacetRaw = 0.0f;
            foreach (var (pgi, evPlatformWeight) in evt.PlatformGroupWeights)
            {
                if (userProfile.PlatformGroupWeights.TryGetValue(pgi, out var pgWeight))
                {
                    float idf = _cache.GetPlatformIdf(pgi);
                    platformFacetRaw += pgWeight * evPlatformWeight * idf;
                }
            }

            float seriesFacetRaw = 0.0f;
            if (userProfile.SeriesWeights.TryGetValue(evt.SeriesId, out var sWeight))
            {
                float idf = _cache.GetSeriesIdf(evt.SeriesId);
                seriesFacetRaw = sWeight * idf;
                result.Labels.Add("Next edition to series from event you follow");
            }

            float wGame = isUpcomingSection ? GameWeightUpcoming : GameWeightArchived;
            float wGenre = isUpcomingSection ? GenreWeightUpcoming : GenreWeightArchived;
            float wPlatform = isUpcomingSection ? PlatformWeightUpcoming : PlatformWeightArchived;
            float wSeries = isUpcomingSection ? SeriesWeightUpcoming : SeriesWeightArchived;

            result.GameFacetContribution = gameFacetRaw * wGame;
            result.GenreFacetContribution = genreFacetRaw * wGenre;
            result.PlatformGroupFacetContribution = platformFacetRaw * wPlatform;
            result.SeriesFacetContribution = gameFacetRaw * wSeries;

            result.PersonalScore = result.GameFacetContribution
                                   + result.GenreFacetContribution
                                   + result.PlatformGroupFacetContribution
                                   + result.SeriesFacetContribution;

            result.PopularityScore = evt.FollowerCount / maxFollowers;

            if (isUpcomingSection)
            {
                int pastFollowers = pastSnapshots.TryGetValue(evt.EventId, out var count) ? count : evt.FollowerCount;
                int delta = evt.FollowerCount - pastFollowers;

                result.MomentumScore = Math.Max(0.0f, (float)delta / (pastFollowers + MomentumSmoothingK));
            }
            else
            {
                result.MomentumScore = 0.0f;
            }

            float alpha = userProfile.Alpha;
            float beta = isUpcomingSection ? BetaMomentumUpcoming : 0.0f;

            result.FinalScore = (alpha * result.PersonalScore)
                                + ((1.0f - alpha) * result.PopularityScore)
                                + (beta * result.MomentumScore);

            results.Add(result);
        }

        return results.OrderByDescending(r => r.FinalScore).ToList();

    }
}