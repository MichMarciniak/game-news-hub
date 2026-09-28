using GameNewsHub.Contracts;
using GameNewsHub.Data.Entities;

namespace GameNewsHub.Api.Mappings;

public static class GameMappingExtension
{
    public static GameDetailDto ToDetailDto(this Game game, List<PlatformGroupWithPlatformsDto> platformGroupDtos)
    {
        return new GameDetailDto
        {
            Id = game.Id,
            Title = game.Title,
            Summary = game.Summary,
            CoverUrl = game.CoverUrl,
            Genres = game.Genres.Select(g => g.ToDto()).ToList(),
            Addons = game.ChildGames.Select(g => g.ToListItemDto()).ToList(),
            PlatformGroups = platformGroupDtos,
            ReleaseDate = game.ReleaseDate,
            Type = (GameTypeString)game.Type,
            ParentGame = game.ParentGame.ToParentGameDto()
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

    public static ParentGameDto? ToParentGameDto(this Game? game)
    {
        if (game == null) return null;
        return new ParentGameDto
        {
            Id = game.Id,
            Title = game.Title,
        };
    }
}