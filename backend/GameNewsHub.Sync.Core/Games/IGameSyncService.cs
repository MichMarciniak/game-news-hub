using GameNewsHub.Data.Entities;

namespace GameNewsHub.Sync.Core.Games;

public interface IGameSyncService
{
    public Task<List<Game>> GetOrCreateGamesAsync(List<int> gameIds);
}