using GameNewsHub.Data.Entities;

namespace GameNewsHub.Sync.Sync.Games;

public interface IGameSyncService
{
    public Task SyncUpcomingGamesAsync(int limit);

    public Task<ICollection<Game>> GetOrCreateGamesAsync(IEnumerable<int> gameIds);
}