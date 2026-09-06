using System.Reflection.Metadata.Ecma335;
using backend.Data;
using ErrorOr;
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
        IQueryable<Event> dbQuery = _context.Events;

        if (startTime != null)
        {
            var unixStartTime = ((DateTimeOffset)startTime);
            dbQuery = dbQuery.Where(e => e.StartTime >= unixStartTime);
        }

        if (endTime != null)
        {
            var unixEndTime = ((DateTimeOffset)endTime);
            dbQuery = dbQuery.Where(e => e.EndTime <= unixEndTime);
        }

        var events = await dbQuery
            .Select(e => e.ToListItemDto())
            .ToListAsync();

        return events;
    }

    public async Task<ErrorOr<EventDetailDto>> GetEventDetails(int eventId)
    {
        var e = await _context.Events
            .Include(e => e.Games)
            .FirstOrDefaultAsync(e => e.Id == eventId);
            
        if (e == null)
        {
            return Error.NotFound("Event.NotFound", $"Event {eventId} not found");
        }

        return e.ToDetailDto();
    }
}