using GameNewsHub.Data.Entities;
using GameNewsHub.Sync.Dtos;

namespace GameNewsHub.Sync.Sync.Platforms;

public interface IPlatformSyncService
{
    public Task<ICollection<Platform>> GetOrCreateBatchAsync(IEnumerable<IgdbPlatformResponse> platformDtos);
}