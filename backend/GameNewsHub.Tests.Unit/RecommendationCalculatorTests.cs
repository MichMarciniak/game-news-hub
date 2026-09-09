using backend.Configuration;
using FluentAssertions;
using GameNewsHub.Api.Features.Recommendations;

namespace GameNewsHub.Tests;

public class RecommendationCalculatorTests
{
    
    
    [Fact]
    public void Calculate_FollowedPlatformGroup_AddsBonus()
    {
        var weights = new RecommendationWeights { FollowedPlatformBonus = 50 };
        var evt = new EventScoringInput(1, "Test", DateTimeOffset.UtcNow.AddDays(5),
            GameIds: new List<int>(),
            PlatformGroupIds: new List<int> { 3 },
            GenreWeights: new List<EventGenreWeightInput>());

        var context = new RecommendationContext(
            FollowedGameIds: new HashSet<int>(),
            FollowedEventIds: new HashSet<int>(),
            FollowedPlatformGroupIds: new HashSet<int> { 3 },
            UserGenreWeights: new Dictionary<int, double>());

        var result = RecommendationCalculator.Calculate(evt, context, weights, DateTimeOffset.UtcNow);

        result.ContainsFollowedPlatformGroup.Should().BeTrue();
        result.FinalPriority.Should().BeGreaterThanOrEqualTo(50);
    }
}