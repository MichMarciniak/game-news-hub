using GameNewsHub.Contracts.Events;
using GameNewsHub.Contracts.Games;
using GameNewsHub.Data.Entities;

namespace GameNewsHub.Api.Features.Events;

public static class EventMappingExtension
{
    public static EventListItemDto ToListItemDto(this Event e)
    {
        return new EventListItemDto
        {
            Id = e.Id,
            Name = e.Name,
            StartTime = e.StartTime,
            EndTime = e.EndTime,
            FollowerCount = e.FollowerCount
        };
    }

    public static EventListNameDto ToListNameDto(this Event e)
    {
        return new EventListNameDto
        {
            Id = e.Id,
            Name = e.Name
        };
    }

    public static EventDetailDto ToDetailDto(
        this Event e,
        List<GameListItemDto> games,
        List<EventListNameDto> relatedEvents)
    {
        return new EventDetailDto
        {
            Id = e.Id,
            Name = e.Name,
            Description = e.Description,
            StartTime = e.StartTime,
            EndTime = e.EndTime,
            Games = games,
            FollowerCount = e.FollowerCount,
            LiveStreamUrl = e.LiveStreamUrl,
            RelatedEvents = relatedEvents
        };
    }
}