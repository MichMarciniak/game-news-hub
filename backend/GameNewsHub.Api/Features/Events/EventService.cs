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

    public async Task<List<EventListNameDto>> SearchEvents(string? query, int page, int pageSize)
    {
        IQueryable<Event> dbQuery = _context.Events;

        if (!string.IsNullOrWhiteSpace(query))
            dbQuery = dbQuery.Where(g => g.Name.ToLower().Trim().Contains(query.ToLower()));

        var events = await dbQuery
            .OrderBy(e => e.Name)
            .Select(e => e.ToListNameDto())
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return events;
    }
}