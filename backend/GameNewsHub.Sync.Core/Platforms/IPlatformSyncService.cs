using GameNewsHub.Data.Entities;
using GameNewsHub.External.Igdb.Dtos;

namespace GameNewsHub.Sync.Core.Platforms;

public interface IPlatformSyncService
{
    public Task<List<Platform>> GetOrCreateBatchAsync(List<IgdbPlatformResponse> platformDtos);
}