using GameNewsHub.Data.Entities;
using GameNewsHub.External.Igdb.Dtos;

namespace GameNewsHub.Sync.Core.Games;

public interface IGameSyncService
{
    public Task<List<Game>> GetOrCreateGamesAsync(List<int> gameIds);

    public Task<List<Game>> ImportByExternalIdsAsync(IEnumerable<string> externalIds, ExternalGameSource source);
}