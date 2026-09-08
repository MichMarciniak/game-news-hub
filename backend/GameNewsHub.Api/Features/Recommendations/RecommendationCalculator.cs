using backend.Configuration;
using GameNewsHub.Contracts;

namespace GameNewsHub.Api.Features.Recommendations;

public record EventScoringInput(
    int EventId,
    string Name,
    DateTimeOffset StartTime,
    IReadOnlyCollection<int> GameIds,
    IReadOnlyCollection<int> PlatformGroupIds,
    IReadOnlyCollection<EventGenreWeightInput> GenreWeights
);

public record EventGenreWeightInput(int GenreId, double Weight);

public record RecommendationContext(
    HashSet<int> FollowedGameIds,
    HashSet<int> FollowedPlatformGroupIds,
    HashSet<int> FollowedEventIds,
    IReadOnlyDictionary<int, double> UserGenreWeights
);

public static class RecommendationCalculator
{
    public static RecommendationDto Calculate(
        EventScoringInput evt,
        RecommendationContext context,
        RecommendationWeights weights,
        DateTimeOffset now)
    {

        bool isFollowed = context.FollowedEventIds.Contains(evt.EventId);
        bool containsFollowedGame = evt.GameIds.Any(id => context.FollowedGameIds.Contains(id));
        bool containsFollowedPlatformGroup =
            evt.PlatformGroupIds.Any(id => context.FollowedPlatformGroupIds.Contains(id));

        double matchScore = evt.GenreWeights.Sum(gw =>
            gw.Weight * context.UserGenreWeights.GetValueOrDefault(gw.GenreId, 0));

        double recencyBoost = GetRecencyBoost(evt.StartTime, now);

        double priority = 0;
        if (isFollowed) priority += weights.FollowedEventBonus;
        if (containsFollowedGame) priority += weights.FollowedGameBonus;
        if (containsFollowedPlatformGroup) priority += weights.FollowedPlatformBonus;
        priority += matchScore * weights.GenreMatchMultiplier;
        priority += recencyBoost;

        return new RecommendationDto
        {
            Id = evt.EventId,
            Name = evt.Name,
            ContainsFollowedGame = containsFollowedGame,
            ContainsFollowedPlatformGroup = containsFollowedPlatformGroup,
            MatchScore = matchScore,
            IsFollowed = isFollowed,
            FinalPriority = priority
        };
    }
    
    private static double GetRecencyBoost(DateTimeOffset startTime, DateTimeOffset now)
    {
        var daysUntil = (startTime - now).TotalDays;
        if (daysUntil <= 7) return 2;
        if (daysUntil <= 30) return 1;
        return 0;
    }
}