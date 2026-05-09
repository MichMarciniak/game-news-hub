using GameNewsHub.Api.Entities;
using GameNewsHub.Contracts;

namespace GameNewsHub.Api.Features.Games;

public static class GameMappings
{
    public static GameListResponse ToListResponse(this Game game)
    {
        return new GameListResponse
        {
            Id = game.Id,
            Name = game.Title
        };
    }
    
    public static GameDetailsResponse ToDetailsResponse(this Game game)
    {
        return new GameDetailsResponse
        {
            Id = game.Id,
            Name = game.Title,
            Summary = game.Summary,
        };
    }
}