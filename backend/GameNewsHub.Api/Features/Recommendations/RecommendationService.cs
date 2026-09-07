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
        /*
         * 1. followowane eventy
         * 2. eventy z followowanymi grami
         * 3. weight
         */

        var user = await _context.Users
            .Include(u => u.FollowedGames)
            .ThenInclude(g => g.Genres)
            .Include(u => u.FollowedEvents)
            .Include(appUser => appUser.FollowedPlatformGroups)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return new List<RecommendationDto>();

        var userGenreWeights = user.FollowedGames
            .SelectMany(g => g.Genres)
            .GroupBy(genre => genre.Id)
            .ToDictionary(g => g.Key, g => (double)g.Count());
        
        var followedGameIds = user.FollowedGames.Select(g => g.Id);
        var followedEventIds = user.FollowedEvents.Select(e => e.Id);
        var followedPlatformGroupIds = user.FollowedPlatformGroups.Select(g => g.Id);

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

        var result = events.Select(e =>
            {
                var platformGroupIds = e.Games
                    .SelectMany(g => g.Platforms)
                    .GroupBy(p => p.PlatformGroup)
                    .Select(pg => pg.Key.Id)
                    .ToList();
                
                bool isFollowed = followedEventIds.Contains(e.Id);
                bool containsFollowedGame = e.Games
                    .Any(g => followedGameIds.Contains(g.Id));
                bool containsFollowedPlatformGroup = platformGroupIds
                    .Any(pg => followedPlatformGroupIds.Contains(pg));

                double matchScore = e.GenreWeights.Sum(gw =>
                    gw.Weight * userGenreWeights.GetValueOrDefault(gw.GenreId, 0));

                double recencyBoost = GetRecencyBoost(e.StartTime, now);

                double priority = 0;
                if (isFollowed) priority += _weights.FollowedEventBonus;
                if (containsFollowedGame) priority += _weights.FollowedGameBonus;
                if (containsFollowedPlatformGroup) priority += _weights.FollowedPlatformBonus;
                priority += (matchScore * _weights.GenreMatchMultiplier);
                priority += recencyBoost;

                return new RecommendationDto 
                {
                    Id = e.Id,
                    Name = e.Name,
                    IsFollowed = isFollowed,
                    ContainsFollowedGame = containsFollowedGame,
                    ContainsFollowedPlatformGroup = containsFollowedPlatformGroup,
                    MatchScore = Math.Round(matchScore * 100, 2), //procentowo
                    FinalPriority = priority
                };
            })
            .OrderByDescending(r => r.FinalPriority)
            .Take(20)
            .ToList();

        return result;
    }

    private double GetRecencyBoost(DateTimeOffset startTime, DateTimeOffset now)
    {
        var daysUntil = (startTime - now).TotalDays;
        if (daysUntil <= 7) return 2;
        if (daysUntil <= 30) return 1;
        return 0;
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
 
    /*
    public async Task<ErrorOr<List<RecommendationDto>>> GetNewRecommendations(int userId)
    {
        var user = await _context.Users
            .AsSplitQuery()
            .Include(u => u.FollowedGames)
            .Include(u => u.FollowedEvents)
            .Include(u => u.FollowedGenres)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return Error.NotFound("User.NotFound", $"User with id {userId} was not found");
        }

        var followedEvents = user.FollowedEvents.ToList();

        var followedGames = user.FollowedGames.ToList();

        var followedGenres = user.FollowedGenres.ToList();

        var events = await _context.Events
            .Include(e => e.GenreWeights)
            .Include(e => e.Games)
            .Where(e => e.Status == EventSyncStatus.Ready)
            .ToListAsync();

        var result = events.Select(e =>
        {
            var followedEvent = false;
            var followedGame = false;
            var followedGenre = false;

            var score = 0;
            if (followedEvents.Contains(e))
            {
                followedEvent = true;
                score += 5;
            }

            score += followedEvents.Contains(e) ? 5 : 0;
            foreach (var game in e.Games)
            {
                if (followedGames.Contains(game))
                {
                    followedGame = true;
                    score += 4;
                    break;
                }
            }

            foreach (var genre in e.GenreWeights)
            {
                if (followedGenres.Select(g => g.Id).Contains(genre.GenreId))
                {
                    followedGenre = true;
                    score += 3;
                    break;
                }
            }

            score = RecencyBoost(e);
            // platform match?
            return new RecommendationDto
            {
                Id = e.Id,
                Name = e.Name,
                ContainsFollowedGame = followedGame,
                Score = score,
                IsFollowed = followedEvent,
                ContainsFollowedGenre = followedGenre
            };

        }).Where(r => r.Score != 0).ToList();

        return result;
    }

    */
}