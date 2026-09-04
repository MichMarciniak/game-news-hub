using GameNewsHub.Api.Features.Genres;
using GameNewsHub.Contracts;
using GameNewsHub.Data.Entities;

namespace GameNewsHub.Api.Mappings;

public static class GameMappingExtension
{
    public static GameDetailDto ToDetailDto(this Game game)
    {
        return new GameDetailDto
        {
            Id = game.Id,
            Title = game.Title,
            Summary = game.Summary,
            CoverUrl = game.CoverUrl,
            Genres = game.Genres.Select(g => g.ToDto()).ToList(),
            Addons = game.ChildGames.Select(g => g.ToListItemDto()).ToList(),
            ParentGameId = game.ParentGameId
        };
    }

    public static GameListItemDto ToListItemDto(this Game game)
    {
        return new GameListItemDto
        {
            Id = game.Id,
            Title = game.Title,
            CoverUrl = game.CoverUrl,
        };
    }
}