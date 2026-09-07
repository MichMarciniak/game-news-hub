using GameNewsHub.Api.Features.Events;
using GameNewsHub.Contracts;
using GameNewsHub.Data.Entities;

namespace GameNewsHub.Api.Mappings;

public static class EventMappingExtension
{
    public static EventListItemDto ToListItemDto(this Event e)
    {
        return new EventListItemDto
        {
            Id = e.Id,
            Name = e.Name,
            StartTime = e.StartTime,
            EndTime = e.EndTime
        };
    }
    public static NormalizedEventListItemDto ToNormalizedListItemDto(this EventListItemDto e)
    {
        return new NormalizedEventListItemDto 
        {
            Id = e.Id,
            Name = e.Name,
            NormalizedName = EventNameNormalizer.Normalize(e.Name),
        };
    }

    public static EventDetailDto ToDetailDto(this Event e)
    {
        return new EventDetailDto
        {
            Id = e.Id,
            Name = e.Name,
            Description = e.Description,
            StartTime = e.StartTime,
            EndTime = e.EndTime,
            Games = e.Games.Select(g => g.ToListItemDto()).ToList()
        };
    }
}