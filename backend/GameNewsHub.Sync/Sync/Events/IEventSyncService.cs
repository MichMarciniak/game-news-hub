namespace GameNewsHub.Sync.Sync.Events;

public interface IEventSyncService
{
    public Task DiscoverNewEventsAsync();

    public Task HydrateEventsAsync();
}