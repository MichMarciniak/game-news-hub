using GameNewsHub.Data.Entities;

namespace GameNewsHub.Api.Configuration;

public static class GameTypePolicy
{
    public static readonly GameType[] MainTypes = new[]
    {
        GameType.MainGame,
        GameType.Remake,
        GameType.Remaster,
        GameType.Fork
    };
}