using backend.Models.Entities;

namespace GameNewsHub.Api.Services.Games;

public interface IGameSyncService
{
    public Task SyncUpcomingGamesAsync(int limit);

    public Task<ICollection<Game>> GetOrCreateGamesAsync(IEnumerable<int> gameIds);
}