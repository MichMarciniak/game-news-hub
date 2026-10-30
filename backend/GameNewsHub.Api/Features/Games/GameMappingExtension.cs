using GameNewsHub.Contracts.Genres;
using GameNewsHub.Contracts.Games;
using GameNewsHub.Contracts.Platforms;
using GameNewsHub.Data.Entities;
using Riok.Mapperly.Abstractions;

namespace GameNewsHub.Api.Features.Games;

public static class GameMappingExtension
{
    public static GameDetailDto ToDetailDto(
        this Game game,
        List<GenreDto> genres,
        List<GameListItemDto> addons,
        List<PlatformGroupWithPlatformsDto> platformGroupDtos,
        ParentGameDto? parentGame)
    {
        return new GameDetailDto
        {
            Id = game.Id,
            Title = game.Title,
            Summary = game.Summary,
            CoverUrl = game.CoverUrl,
            Genres = genres,
            Addons = addons,
            PlatformGroups = platformGroupDtos,
            ReleaseDate = game.ReleaseDate,
            Type = (GameTypeString)game.Type,
            ParentGame = parentGame
        };
    }

    public static GameListItemDto ToListItemDto(this Game game)
    {
        return new GameListItemDto
        {
            Id = game.Id,
            Title = game.Title,
            CoverUrl = game.CoverUrl
        };
    }

    public static ParentGameDto ToParentGameDto(this Game game)
    {
        return new ParentGameDto
        {
            Id = game.Id,
            Title = game.Title,
        };
    }
}

[Mapper]
public static partial class GameMapper
{
    public static partial IQueryable<GameListItemDto> ProjectToListItemDto(this IQueryable<Game> query);

    public static partial IQueryable<GameDetailDto> ProjectToDetailsDto(this IQueryable<Game> query);
}