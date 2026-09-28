using System.Diagnostics;
using ErrorOr;
using GameNewsHub.Api.Features.Shared;
using GameNewsHub.Api.Mappings;
using GameNewsHub.Contracts;
using GameNewsHub.Data;
using GameNewsHub.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameNewsHub.Api.Features.Events;

public class EventService
{
    private readonly AppDbContext _context;
    private readonly ILogger<EventService> _logger;

    public EventService(AppDbContext context, ILogger<EventService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<EventListItemDto>> GetEvents(DateTimeOffset? from, DateTimeOffset? to)
    {
        var now = DateTimeOffset.UtcNow;
        var (rangeFrom, rangeTo) = DateCalculator.GetDateRange(from, to);

        var events = await _context.Events
            .Where(e => e.StartTime <= rangeTo)
            .Where(e => (e.EndTime ?? e.StartTime) >= rangeFrom)
            .Select(e => e.ToListItemDto())
            .ToListAsync();

        return events;
    }

    public async Task<List<NormalizedEventListItemDto>> GetNormalizedEvents(DateTime? startTime, DateTime? endTime)
    {
        var events = await GetEvents(startTime, endTime);

        return events.Select(e => e.ToNormalizedListItemDto()).ToList();
    }

    public async Task<ErrorOr<EventDetailDto>> GetEventDetails(int eventId)
    {
        var e = await _context.Events
            .AsSplitQuery()
            .Include(e => e.Games)
            .Include(e => e.Series)
            .ThenInclude(s => s.Events)
            .FirstOrDefaultAsync(e => e.Id == eventId);

        if (e == null) return Error.NotFound("Event.NotFound", $"Event {eventId} not found");

        return e.ToDetailDto();
    }

    public async Task<ErrorOr<Success>> ReassignEventToSeries(int eventId, int seriesId)
    {
        var evt = await _context.Events
            .Include(e => e.Series)
            .FirstOrDefaultAsync(e => e.Id == eventId);

        if (evt == null)
        {
            _logger.LogWarning(
                "Reassign failed: Event {EventId} not found",
                eventId);
            return Error.NotFound("Event.NotFound", $"Event {eventId} not found");
        }

        var series = await _context.EventSeries
            .FirstOrDefaultAsync(s => s.Id == seriesId);

        if (series == null)
        {
            _logger.LogWarning(
                "Reassign failed: EventSeries {SeriesId} not found",
                seriesId);
            return Error.NotFound("EventSeries.NotFound", $"EventSeries {seriesId} not found");
        }

        var oldSeriesId = evt.EventSeriesId;
        evt.Series = series;
        await _context.SaveChangesAsync();
        
        _logger.LogInformation(
            "Event {EventId} reassigned from series {OldSeriesId} to {NewSeriesId}",
            evt.Id, oldSeriesId, seriesId);

        return new ErrorOr<Success>();
    }

    public async Task<SearchDto<EventListItemDto>> SearchEvents(
        string? query, int page, int pageSize,
        EventSortBy? sortBy, bool descending = true)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 50);
        
        IQueryable<Event> dbQuery = _context.Events.AsQueryable();

        var hasQuery = !string.IsNullOrWhiteSpace(query);
        var cleanQuery = string.Empty;

        if (hasQuery)
        {
            cleanQuery = query!.Trim();
            dbQuery = dbQuery
                .Where(e => EF.Functions.ILike(e.Name, $"%{cleanQuery}%"));
        }

        var total = await dbQuery.CountAsync();

        if (total == 0)
        {
            return new SearchDto<EventListItemDto>
            {
                Total = 0,
                Items = new()
            };
        }
        
        IQueryable<Event> sortedQuery;

        if (!hasQuery)
        {
            sortedQuery = dbQuery.ApplySorting(sortBy ?? EventSortBy.Date, descending);
        }
        else if (sortBy.HasValue && sortBy.Value != EventSortBy.Relevance)
        {
            sortedQuery = dbQuery.ApplySorting(sortBy.Value, descending);
        }
        else
        {
            sortedQuery = dbQuery 
                .OrderByDescending(e => EF.Functions.ILike(e.Name, $"%{cleanQuery}%"))
                .ThenByDescending(e => EF.Functions.TrigramsSimilarity(e.Name, cleanQuery))
                .ThenBy(e => EF.Functions.FuzzyStringMatchLevenshteinLessEqual(e.Name, cleanQuery, 5));
        }

        var items = await sortedQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => e.ToListItemDto())
            .ToListAsync();

        return new SearchDto<EventListItemDto>
        {
            Total = total,
            Items = items
        };
    }

    public async Task<SearchDto<AdminSeriesDto>> GetAdminSeries(string? query, int page, int pageSize)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 50);

        IQueryable<EventSeries> dbQuery = _context.EventSeries
            .Where(s => s.Events.Any());

        if (!string.IsNullOrWhiteSpace(query))
        {
            query = query.Trim();
            dbQuery = dbQuery
                .Where(s => s.Events
                    .Any(e => EF.Functions.ILike(e.Name, $"%{query}%")));

        };

        var total = await dbQuery.CountAsync();

        var items = await dbQuery
            .OrderByDescending(s => s.Events.Max(e => e.StartTime))
            .ThenBy(s => s.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new AdminSeriesDto
            {
                Id = s.Id,
                DisplayName = s.Events
                    .OrderByDescending(e => e.StartTime)
                    .Select(e => e.NormalizedName)
                    .FirstOrDefault() ?? "",
                Events = s.Events.Select(e => e.ToListItemDto()).ToList()
            })
            .ToListAsync();

        return new SearchDto<AdminSeriesDto>
        {
            Total = total,
            Items = items
        };
    }
}