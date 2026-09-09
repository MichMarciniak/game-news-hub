using backend.Configuration;
using backend.Data;
using GameNewsHub.Api.Features.Shared;
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

    private async Task<AppUser?> LoadUserWithFollows(int userId)
    {
        return await _context.Users
            .Include(u => u.FollowedGames).ThenInclude(g => g.Genres)
            .Include(u => u.FollowedEvents)
            .Include(appUser => appUser.FollowedPlatformGroups)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    private (DateTimeOffset From, DateTimeOffset To) GetDateRange(DateTime? from, DateTime? to)
    {
        var now = DateTimeOffset.UtcNow;
        var (defaultFrom, defaultTo) = DateCalculator.GetDefaultRange(now);
        var rangeFrom = from ?? defaultFrom;
        var rangeTo = to ?? defaultTo;
        return (rangeFrom, rangeTo);
    }

    private List<EventSyncStatus> GetStatuses(bool includeUncertain)
    {
        var statuses = new List<EventSyncStatus> { EventSyncStatus.Ready };
        if (includeUncertain)
        {
            statuses.Add(EventSyncStatus.NoData);
            statuses.Add(EventSyncStatus.Pending);
        }

        return statuses;
    }

    private RecommendationContext BuildRecommendationContext(AppUser user)
    {
        var userGenreWeights = user.FollowedGames
            .SelectMany(g => g.Genres)
            .GroupBy(genre => genre.Id)
            .ToDictionary(g => g.Key, g => (double)g.Count());
        
        return new RecommendationContext(
            FollowedGameIds: user.FollowedGames.Select(g => g.Id).ToHashSet(),
            FollowedEventIds: user.FollowedEvents.Select(e => e.Id).ToHashSet(),
            FollowedPlatformGroupIds: user.FollowedPlatformGroups.Select(g => g.Id).ToHashSet(),
            UserGenreWeights: userGenreWeights
        );
    }
    
    public async Task<List<RecommendationDto>> GetRecommendedEventList(int userId, DateTime? from = null, DateTime? to = null)
    {

        var user = await LoadUserWithFollows(userId);
        if (user == null) return new List<RecommendationDto>();

        var now = DateTimeOffset.UtcNow;
        var (rangeFrom, rangeTo) = GetDateRange(from, to);

        var statuses = GetStatuses(user.ShowFutureRecommendations);

        var events = await _context.Events
            .AsSplitQuery()
            .Include(e => e.GenreWeights)
            .Include(e => e.Games)
                .ThenInclude(g => g.Platforms)
                    .ThenInclude(p => p.PlatformGroup)
            .Where(e => e.StartTime >= rangeFrom && e.StartTime <= rangeTo)
            .Where(e => statuses.Contains(e.Status))
            .ToListAsync();

        var uncertainSeriesIds = events
            .Where(e => e.Status != EventSyncStatus.Ready)
            .Select(e => e.EventSeriesId)
            .ToList();

        // bierze eventy z serii które mają już dane
        // bierze ich genre weights
        // i przypisuje do danego eventu (tego bez danych)
        var predictedWeightsBySeriesId = uncertainSeriesIds.Any()
            ? await _context.Events
                .Where(e => uncertainSeriesIds.Contains(e.EventSeriesId))
                .Where(e => e.Status == EventSyncStatus.Ready)
                .Include(e => e.GenreWeights)
                .GroupBy(e => e.EventSeriesId)
                .Select(g => g.OrderByDescending(e => e.StartTime).First())
                .ToDictionaryAsync(e => e.EventSeriesId, e => e.GenreWeights)
            : new Dictionary<int, ICollection<EventGenreWeight>>();

        var context = BuildRecommendationContext(user);

        var result = events
            .Select(e =>
            {
                bool isPredicted = e.Status != EventSyncStatus.Ready;

                var genreWeights = isPredicted
                    ? (predictedWeightsBySeriesId.TryGetValue(e.EventSeriesId, out var w))
                        ? w
                        : new List<EventGenreWeight>()
                    : e.GenreWeights;

                var input = new EventScoringInput(
                    EventId: e.Id,
                    Name: e.Name,
                    StartTime: e.StartTime,
                    GameIds: e.Games.Select(g => g.Id).ToList(),
                    PlatformGroupIds: e.Games
                        .SelectMany(g => g.Platforms)
                        .Select(p => p.PlatformGroupId)
                        .Distinct()
                        .ToList(),
                    GenreWeights: genreWeights
                        .Select(gw => new EventGenreWeightInput(gw.GenreId, gw.Weight))
                        .ToList()
                );

                var result = RecommendationCalculator.Calculate(input, context, _weights, now);
                return result with { IsPredicted = isPredicted };
            })
            .OrderByDescending(r => r.FinalPriority)
            .ToList();
            
        return result;
    }

}