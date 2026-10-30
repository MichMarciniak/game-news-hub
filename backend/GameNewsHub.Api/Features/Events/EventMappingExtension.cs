using GameNewsHub.Api.Features.Games;
using GameNewsHub.Contracts.Games;
using GameNewsHub.Contracts.Events;
using GameNewsHub.Data.Entities;
using Riok.Mapperly.Abstractions;
using System.Linq;

namespace GameNewsHub.Api.Features.Events;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[UseStaticMapper(typeof(GameMapper))]
public static partial class EventMapper
{
    [MapperIgnoreTarget(nameof(EventDetailDto.RelatedEvents))]
    public static partial EventDetailDto ToDetailDto(Event source);

    public static partial EventListItemDto ToListItemDto(Event source);
    public static partial EventListNameDto ToListNameDto(Event source);
    public static partial IQueryable<EventListItemDto> ProjectToListItemDto(this IQueryable<Event> query);
}