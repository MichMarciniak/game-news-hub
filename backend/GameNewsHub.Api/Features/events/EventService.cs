using System.Reflection.Metadata.Ecma335;
using backend.Data;
using ErrorOr;
using GameNewsHub.Api.Features.Shared;
using GameNewsHub.Api.Mappings;
using GameNewsHub.Contracts;
using GameNewsHub.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameNewsHub.Api.Features.Events;

public class EventService
{
    private readonly AppDbContext _context;

    public EventService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<EventListItemDto>> GetEvents(DateTime? startTime, DateTime? endTime)
    {
        var now = DateTimeOffset.UtcNow;
        DateTimeOffset start;
        if (startTime != null)
        {
            start = ((DateTimeOffset)startTime);
        }
        else
        {
            start = DateCalculator.GetMonthStart(now);
        }

        DateTimeOffset end;
        if (endTime != null)
        {
            end = ((DateTimeOffset)endTime);
        }
        else
        {
            end = DateCalculator.GetMonthEnd(now);
        }

        var events = await _context.Events 
            .Where(e => e.StartTime >= start)
            .Where(e => e.StartTime <= end)
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
            
        if (e == null)
        {
            return Error.NotFound("Event.NotFound", $"Event {eventId} not found");
        }

        return e.ToDetailDto();
    }

    public async Task<ErrorOr<Success>> ReassignEventToSeries(int eventId, int seriesId)
    {
        var evt = await _context.Events
            .Include(e => e.Series)
            .FirstOrDefaultAsync(e => e.Id == eventId);

        if (evt == null)
            return Error.NotFound("Event.NotFound", $"Event {eventId} not found");

        var series = await _context.EventSeries
            .FirstOrDefaultAsync(s => s.Id == seriesId);

        if (series == null)
            return Error.NotFound("EventSeries.NotFound", $"EventSeries {seriesId} not found");

        evt.Series = series;
        await _context.SaveChangesAsync();

        return new ErrorOr<Success>();
    }

    public async Task<List<EventListNameDto>> SearchEvents(string? query, int page, int pageSize)
    {
        IQueryable<Event> dbQuery = _context.Events;

        if (!string.IsNullOrWhiteSpace(query))
        {
            dbQuery = dbQuery.Where(g => g.Name.ToLower().Trim().Contains(query.ToLower()));
        }

        var events = await dbQuery
            .OrderBy(e => e.Name)
            .Select(e => e.ToListNameDto())
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return events;
    }

}