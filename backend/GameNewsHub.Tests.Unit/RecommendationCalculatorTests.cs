using FluentAssertions;
using GameNewsHub.Api.Features.Recommendations;
using GameNewsHub.Api.Options;

namespace GameNewsHub.Tests.Unit;

public class RecommendationCalculatorTests
{
    [Fact]
    public void Calculate_FollowedPlatformGroup_AddsBonus()
    {
        var weights = new RecommendationWeights { FollowedPlatformBonus = 50 };
        var evt = new EventScoringInput(1, "Test", DateTimeOffset.UtcNow.AddDays(5),
            new List<int>(),
            new List<int> { 3 },
            new List<EventGenreWeightInput>());

        var context = new RecommendationContext(
            new HashSet<int>(),
            FollowedEventIds: new HashSet<int>(),
            FollowedPlatformGroupIds: new HashSet<int> { 3 },
            UserGenreWeights: new Dictionary<int, double>());

        var result = RecommendationCalculator.Calculate(evt, context, weights, DateTimeOffset.UtcNow);

        result.ContainsFollowedPlatformGroup.Should().BeTrue();
        result.FinalPriority.Should().BeGreaterThanOrEqualTo(50);
    }
}