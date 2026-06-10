using GameNewsHub.Api.Entities;
using GameNewsHub.Contracts;

namespace GameNewsHub.Api.Sync.Platforms;

public interface IPlatformSyncService
{
    public Task<ICollection<Platform>> GetOrCreateBatchAsync(IEnumerable<PlatformDto> platformDtos);
}