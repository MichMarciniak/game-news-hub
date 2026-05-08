using GameNewsHub.Api.Entities;

namespace GameNewsHub.Api.Sync.Games;

public interface IGameSyncService
{
    public Task SyncUpcomingGamesAsync(int limit);

    public Task<ICollection<Game>> GetOrCreateGamesAsync(IEnumerable<int> gameIds);
}