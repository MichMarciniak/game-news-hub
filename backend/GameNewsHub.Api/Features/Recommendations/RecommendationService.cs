using System.Diagnostics;
using GameNewsHub.Api.Features.Shared;
using GameNewsHub.Api.Options;
using GameNewsHub.Contracts.Recommendations;
using GameNewsHub.Data;
using GameNewsHub.Data.Entities;
using GameNewsHub.Recommendation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GameNewsHub.Api.Features.Recommendations;

public class RecommendationService
{
    private readonly UserProfileBuilder _profileBuilder;
    private readonly EventScoringEngine _engine;
    private readonly EventVectorRepresenter _representer;
    private readonly RecMapper _mapper;
    private readonly AppDbContext _context;
    private readonly ILogger<RecommendationService> _logger;
    private readonly RecommendationOptions _options;

    public RecommendationService(
        UserProfileBuilder profileBuilder,
        EventScoringEngine engine,
        EventVectorRepresenter representer,
        RecMapper mapper,
        AppDbContext context,
        ILogger<RecommendationService> logger,
        IOptions<RecommendationOptions> options)
    {
        _profileBuilder = profileBuilder;
        _engine = engine;
        _representer = representer;
        _mapper = mapper;
        _context = context;
        _logger = logger;
        _options = options.Value;
    }

    /// <param name="userId">null = uzytkownik niezalogowany (popularnosc + momentum)</param>
    /// <param name="from">poczatek zakresu (kalendarz: zwykle poczatek miesiaca)</param>
    /// <param name="to">koniec zakresu</param>
    /// <param name="limit">Maksymalna liczba wyników (null = domyślna z config)</param>
    public async Task<RecommendationResponse> GetRecommendationsAsync(
        int? userId,
        DateTimeOffset? from,
        DateTimeOffset? to,
        int? limit = null,
        CancellationToken ct = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var now = DateTimeOffset.UtcNow;

        // ten sam zakres i te same limity co w zwyklej liscie eventow
        var (rangeFrom, rangeTo) = DateCalculator.GetDateRange(from, to);

        var profile = userId.HasValue
            ? await _profileBuilder.BuildProfileAsync(userId.Value, ct)
            : new UserProfileVector { Alpha = 0f };

        // eventy, ktore uzytkownik juz sledzi, nie sa rekomendowane
        var followedEventIds = userId.HasValue
            ? await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == userId.Value)
                .SelectMany(u => u.FollowedEvents)
                .Select(e => e.Id)
                .ToListAsync(ct)
            : new List<int>();

        // kandydaci: eventy zachodzace na zakres (jak w GetEvents)
        var events = await EventsWithGames()
            .Where(e => !followedEventIds.Contains(e.Id))
            .Where(e => e.StartTime <= rangeTo)
            .Where(e => (e.EndTime ?? e.StartTime) >= rangeFrom)
            .ToListAsync(ct);

        if (events.Count == 0)
        {
            return new RecommendationResponse { Personalization = profile.Alpha };
        }

        // globalny max followerow: popularnosc ma byc porownywalna miedzy miesiacami
        var maxFollowers = await _context.Events
            .AsNoTracking()
            .MaxAsync(e => (int?)e.FollowerCount, ct) ?? 1;

        var eventsById = events.ToDictionary(e => e.Id);

        // sekcja jest per event: trwajace i przyszle -> wagi "upcoming", zakonczone -> "archived"
        var upcoming = events.Where(e => (e.EndTime ?? e.StartTime) > now).ToList();
        var archived = events.Where(e => (e.EndTime ?? e.StartTime) <= now).ToList();

        // Use Personalized mode: pure personal score, no popularity, no momentum
        var upcomingScored = await ScoreGroupAsync(profile, upcoming, isUpcoming: true, maxFollowers, ScoringMode.Personalized, ct);
        var archivedScored = await ScoreGroupAsync(profile, archived, isUpcoming: false, maxFollowers, ScoringMode.Personalized, ct);

        // Combine and sort all scored events
        var allScored = upcomingScored.Select(r => (Result: r, IsUpcoming: true))
            .Concat(archivedScored.Select(r => (Result: r, IsUpcoming: false)))
            .OrderByDescending(x => x.Result.FinalScore)
            .ToList();

        // Hybrid threshold + minResults: first filter by threshold, then ensure minimum results
        var thresholdFiltered = allScored.Where(x => x.Result.FinalScore >= _options.Threshold).ToList();
        var minResults = _options.MinResults;

        var finalResults = thresholdFiltered.Count >= minResults
            ? thresholdFiltered
            : allScored.Take(Math.Max(minResults, thresholdFiltered.Count)).ToList();

        // Apply user limit (or default)
        var effectiveLimit = limit ?? _options.DefaultLimit;
        finalResults = finalResults.Take(effectiveLimit).ToList();

        var dtos = finalResults.Select(x =>
        {
            var dto = _mapper.MapToRecommendation(x.Result, eventsById[x.Result.EventVector.EventId]);
            dto.IsUpcoming = x.IsUpcoming;
            return dto;
        }).ToList();

        _logger.LogInformation(
            "Recommendations for user {UserId}: {Candidates} candidates, {ThresholdFiltered} above threshold, {Returned} returned, alpha {Alpha:F2}, {Ms} ms",
            userId, allScored.Count, thresholdFiltered.Count, dtos.Count, profile.Alpha, stopwatch.ElapsedMilliseconds);

        return new RecommendationResponse
        {
            Personalization = profile.Alpha,
            Events = dtos
        };
    }

    private async Task<List<ScoredEventResult>> ScoreGroupAsync(
        UserProfileVector profile,
        List<Event> events,
        bool isUpcoming,
        int maxFollowers,
        ScoringMode mode,
        CancellationToken ct)
    {
        if (events.Count == 0) return [];

        var vectors = await _representer.BuildEventVectorListAsync(events, ct);
        var scored = await _engine.ScoreEventsAsync(profile, vectors, isUpcoming, maxFollowers, mode, ct);

        return scored;
    }

    private IQueryable<Event> EventsWithGames() =>
        _context.Events
            .AsNoTracking()
            .AsSplitQuery()
            .Include(e => e.Games).ThenInclude(g => g.Genres)
            .Include(e => e.Games).ThenInclude(g => g.Platforms);
}