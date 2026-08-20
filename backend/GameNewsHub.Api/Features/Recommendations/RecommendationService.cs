using backend.Data;
using GameNewsHub.Api.Dtos;
using GameNewsHub.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameNewsHub.Api.Features.Recommendations;

public class RecommendationService : IRecommendationService
{
    private readonly AppDbContext _context;

    public RecommendationService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<EventRecommendationResponse>> GetRecommendedEventList(int userId)
    {
        /*
         * 1. followowane eventy
         * 2. eventy z followowanymi grami
         * 3. weight
         */

        var userWeights = await _context.UserGenreWeights
            .Where(w => w.UserId == userId)
            .ToDictionaryAsync(w => w.GenreId, w => w.Weight);

        var followedGameIds = await _context.Users
            .Where(u => u.Id == userId)
            .SelectMany(u => u.FollowedGames)
            .Select(g => g.Id)
            .ToListAsync();

        var followedEventsIds = await _context.Users
            .Where(u => u.Id == userId)
            .SelectMany(u => u.FollowedEvents)
            .Select(e => e.Id)
            .ToListAsync();

        var events = await _context.Events
            .Include(e => e.GenreWeights)
            .Include(e => e.Games)
            .Where(e => e.Status == EventSyncStatus.Ready)
            .ToListAsync();

        var result = events.Select(e =>
            {
                bool isFollowed = followedEventsIds.Contains(e.Id);
                bool containsFollowedGame = e.Games.Any(g => followedGameIds.Contains(g.Id));

                double matchScore = 0;
                foreach (var gw in e.GenreWeights)
                {
                    var uw = userWeights.TryGetValue(gw.GenreId, out var value) ? value : 0;
                    matchScore += gw.Weight * uw;
                }

                double priority = 0;
                if (isFollowed) priority += 1000;
                if (containsFollowedGame) priority += 100;
                priority += (matchScore * 10);

                return new EventRecommendationResponse
                {
                    Id = e.Id,
                    Name = e.Name,
                    IsFollowed = isFollowed,
                    ContainsFollowedGame = containsFollowedGame,
                    MatchScore = Math.Round(matchScore * 100, 2), //procentowo
                    FinalPriority = priority
                };
            })
            .OrderByDescending(r => r.FinalPriority)
            .ToList();

        return result;
    }

    /*
    public async Task<List<EventResponse>> GetDefaultEventsList()
    {
        /*
         * pobiera po liczbie obserwowanych eventów/polubieniach
         * 
         #1#
        throw new NotImplementedException();
    }
    */
    
    
}