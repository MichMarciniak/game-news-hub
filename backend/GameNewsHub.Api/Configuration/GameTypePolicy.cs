using Data.Entities;

namespace backend.Configuration;

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