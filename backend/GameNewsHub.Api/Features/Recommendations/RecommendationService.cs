using backend.Configuration;
using backend.Data;
using GameNewsHub.Contracts;
using GameNewsHub.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GameNewsHub.Api.Features.Recommendations;

public class RecommendationService
{
    private readonly AppDbContext _context;
    private readonly RecommendationWeights _weights;

    public RecommendationService(AppDbContext context, IOptions<RecommendationWeights> options)
    {
        _context = context;
        _weights = options.Value;
    }
    
    public async Task<List<RecommendationDto>> GetRecommendedEventList(int userId)
    {

        var user = await _context.Users
            .Include(u => u.FollowedGames).ThenInclude(g => g.Genres)
            .Include(u => u.FollowedEvents)
            .Include(appUser => appUser.FollowedPlatformGroups)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return new List<RecommendationDto>();

        var userGenreWeights = user.FollowedGames
            .SelectMany(g => g.Genres)
            .GroupBy(genre => genre.Id)
            .ToDictionary(g => g.Key, g => (double)g.Count());
        
        var context = new RecommendationContext(
            FollowedGameIds: user.FollowedGames.Select(g => g.Id).ToHashSet(),
            FollowedEventIds: user.FollowedEvents.Select(e => e.Id).ToHashSet(),
            FollowedPlatformGroupIds: user.FollowedPlatformGroups.Select(g => g.Id).ToHashSet(),
            UserGenreWeights: userGenreWeights
        );

        var now = DateTimeOffset.UtcNow;

        var events = await _context.Events
            .AsSplitQuery()
            .Include(e => e.GenreWeights)
            .Include(e => e.Games)
                .ThenInclude(g => g.Platforms)
                    .ThenInclude(p => p.PlatformGroup)
            .Where(e => e.StartTime <= now)
            .Where(e => e.Status == EventSyncStatus.Ready)
            .ToListAsync();

        var result = events
            .Select(e => RecommendationCalculator.Calculate(
                new EventScoringInput(
                    EventId: e.Id,
                    Name: e.Name,
                    StartTime: e.StartTime,
                    GameIds: e.Games.Select(g => g.Id).ToList(),
                    PlatformGroupIds: e.Games
                        .SelectMany(g => g.Platforms)
                        .Select(p => p.PlatformGroupId)
                        .Distinct()
                        .ToList(),
                    GenreWeights: e.GenreWeights
                        .Select(gw => new EventGenreWeightInput(gw.GenreId, gw.Weight))
                        .ToList()
                ),
                context,
                _weights,
                now))
            .OrderByDescending(x => x.FinalPriority)
            .Take(20)
            .ToList();

        return result;
    }

}