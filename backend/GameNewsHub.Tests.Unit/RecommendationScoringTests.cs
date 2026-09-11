using FluentAssertions;
using GameNewsHub.Api.Features.Recommendations;
using GameNewsHub.Api.Options;
using GameNewsHub.Data;
using GameNewsHub.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Xunit.Abstractions;

namespace GameNewsHub.Tests.Unit;

public class RecommendationScoringTests : DatabaseTestBase
{
    private readonly AppDbContext _context;
    private readonly RecommendationService _rs;
    private readonly ITestOutputHelper _testOutputHelper;

    public RecommendationScoringTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _context = Context;
        _rs = new RecommendationService(Context, GetWeights());
        SeedTestData();
    }

    private IOptions<RecommendationWeights> GetWeights()
    {
        var weights = new RecommendationWeights
        {
            FollowedEventBonus = 5,
            FollowedGameBonus = 4,
            FollowedPlatformBonus = 3,
            GenreMatchMultiplier = 10
        };
        var opt = Options.Create(weights);
        return opt;
    }

    private void SeedTestData()
    {
        var pg = new PlatformGroup
        {
            Id = 1,
            Name = "TP"
        };
        var platform = new Platform
        {
            Id = 1,
            IgdbId = 1,
            Name = "Test platform",
            PlatformGroup = pg
        };

        var genre = new Genre
        {
            Id = 1,
            IgdbId = 1,
            Name = "Test genre"
        };

        var game = new Game
        {
            Id = 1,
            IgdbId = 1,
            Title = "Test game",
            Summary = "Test game summary",
            Genres = new[] { genre },
            Platforms = new[] { platform }
        };

        var evt = new Event
        {
            Id = 1,
            IgdbId = 1,
            Name = "Test event",
            Description = "Test event description",
            NormalizedName = "Test event",
            StartTime = DateTimeOffset.UtcNow.AddDays(-31),
            Games = new[] { game },
            Status = EventSyncStatus.Ready
        };

        var evt2 = new Event
        {
            Id = 2,
            IgdbId = 2,
            Name = "Empty event",
            Description = "Empty event description",
            NormalizedName = "Empty event",
            StartTime = DateTimeOffset.UtcNow.AddDays(-31),
            Status = EventSyncStatus.NoData
        };

        var user = new AppUser
        {
            Id = 2
        };

        Context.AddRange(evt, evt2, game, genre, platform, pg, user);
        Context.SaveChanges();
    }

    [Fact]
    public async Task FollowedGame_ShouldGiveHighPriority()
    {
        //Arrange
        var user = await _context.Users
            .Include(u => u.FollowedGames)
            .FirstOrDefaultAsync(u => u.Id == 2);

        var game = await _context.Games.FirstOrDefaultAsync(g => g.Id == 1);
        user.FollowedGames.Add(game);
        await _context.SaveChangesAsync();

        var eventList = await _rs.GetRecommendedEventList(2);

        var priority = eventList
            .Find(e => e.Id == 1)!
            .FinalPriority;

        priority.Should().BeGreaterThanOrEqualTo(4);
    }

    [Fact]
    public async Task EmptyEvent_ShouldNotShow()
    {
        var eventList = await _rs.GetRecommendedEventList(2);

        eventList.Should().NotContain(e => e.Id == 2);
    }

    [Fact]
    public async Task FollowedPlatform_ShouldGivePriority()
    {
        var user = await _context.Users
            .Include(u => u.FollowedPlatformGroups)
            .FirstOrDefaultAsync(u => u.Id == 2);

        var pg = await _context.PlatformGroups.FirstOrDefaultAsync(p => p.Id == 1);

        user.FollowedPlatformGroups.Add(pg);
        await _context.SaveChangesAsync();


        var eventList = await _rs.GetRecommendedEventList(2);

        var priority = eventList
            .Find(e => e.Id == 1)!
            .FinalPriority;

        _testOutputHelper.WriteLine(priority.ToString());
        priority.Should().BeGreaterThanOrEqualTo(4); //why 5
    }
}