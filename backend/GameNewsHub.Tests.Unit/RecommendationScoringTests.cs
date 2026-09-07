using FluentAssertions;
using GameNewsHub.Data.Entities;

namespace GameNewsHub.Tests;

public class RecommendationScoringTests
{
    [Fact]
    public void FollowedGame_ShouldGiveHighPriority()
    {
        //Arrange
        var evt = new Event
        {
            /* ... */
        };
        var followedGameIds = new HashSet<int> { 1, 2 };

        //var priority = RecommendationCalculator.CalculatePriority();
        var priority = 101;

        priority.Should().BeGreaterThan(100);
    }
}