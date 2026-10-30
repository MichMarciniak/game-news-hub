using GameNewsHub.Contracts.Events;
using GameNewsHub.Data.Entities;

namespace GameNewsHub.Api.Features.Events;

public static class EventQueryExtension
{
    public static IQueryable<Event> ApplySorting(this IQueryable<Event> query, EventSortBy sortBy, bool descending)
    {
        
        return (sortBy, descending) switch
        {
            (EventSortBy.Date, true) => query.OrderByDescending(e => e.StartTime),
            (EventSortBy.Date, false) => query.OrderBy(e => e.StartTime),
            (EventSortBy.Name, true) => query.OrderByDescending(e => e.Name),
            (EventSortBy.Name, false) => query.OrderBy(e => e.Name),
            (EventSortBy.Popularity, true) => query.OrderByDescending(e => e.FollowerCount),
            (EventSortBy.Popularity, false) => query.OrderBy(e => e.FollowerCount),
            _ => query
        };
    }
}