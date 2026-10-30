using System.Diagnostics;
using ErrorOr;
using GameNewsHub.Api.Features.Games;
using GameNewsHub.Api.Features.Shared;
using GameNewsHub.Contracts;
using GameNewsHub.Contracts.Events;
using GameNewsHub.Contracts.Shared;
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
            .ProjectToListItemDto()
            .ToListAsync();

        return events;
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

        var relatedEvents = e.Series?.Events
            .Where(se => se.Id != e.Id)
            .Select(EventMapper.ToListNameDto)
            .ToList() ?? [];

        var result = EventMapper.ToDetailDto(e);
        result.RelatedEvents = relatedEvents;
        return result;
    }

    public async Task<SearchDto<EventListItemDto>> SearchEvents(
        string? query, int page, int pageSize,
        EventSortBy? sortBy, bool descending = true)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 50);

        IQueryable<Event> dbQuery = _context.Events.AsQueryable();

        var hasQuery = !string.IsNullOrWhiteSpace(query);
        var cleanQuery = query?.Trim();
        string like = "", exact = "", prefix = "", wordStart = "";
        if (hasQuery)
        {
            exact = cleanQuery;
            prefix = cleanQuery + "%";
            wordStart = "% " + cleanQuery + "%";
            like = "%" + cleanQuery + "%";

            dbQuery = dbQuery.Where(g =>
                EF.Functions.ILike(g.Name, like) ||
                EF.Functions.TrigramsAreWordSimilar(g.Name, cleanQuery));
        }

        var total = await dbQuery.CountAsync();

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
                .OrderBy(g =>
                    EF.Functions.ILike(g.Name, exact) ? 0 :
                    EF.Functions.ILike(g.Name, prefix) ? 1 :
                    EF.Functions.ILike(g.Name, wordStart) ? 2 :
                    EF.Functions.ILike(g.Name, like) ? 3 : 4)
                .ThenByDescending(g => EF.Functions.TrigramsWordSimilarity(cleanQuery, g.Name))
                .ThenByDescending(g => g.FollowerCount);
        }

        var items = await sortedQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectToListItemDto()
            .ToListAsync();

        return new SearchDto<EventListItemDto>
        {
            Total = total,
            Items = items
        };
    }
}