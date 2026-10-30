using GameNewsHub.Contracts.Genres;
using GameNewsHub.Contracts.Games;
using GameNewsHub.Contracts.Platforms;
using GameNewsHub.Data.Entities;
using Riok.Mapperly.Abstractions;
using System.Linq;
using GameNewsHub.Api.Features.Genres;
using GameNewsHub.Api.Features.Platforms;

namespace GameNewsHub.Api.Features.Games;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[UseStaticMapper(typeof(GenreMapper))]
[UseStaticMapper(typeof(PlatformMapper))]
public static partial class GameMapper
{
    [MapperIgnoreTarget(nameof(GameDetailDto.Genres))]
    [MapperIgnoreTarget(nameof(GameDetailDto.Addons))]
    [MapperIgnoreTarget(nameof(GameDetailDto.PlatformGroups))]
    [MapperIgnoreTarget(nameof(GameDetailDto.ParentGame))]
    public static partial GameDetailDto ToDetailDto(Game source);

    public static partial GameListItemDto ToListItemDto(Game source);
    public static partial ParentGameDto ToParentGameDto(Game source);
    public static partial IQueryable<GameListItemDto> ProjectToListItemDto(this IQueryable<Game> query);
}