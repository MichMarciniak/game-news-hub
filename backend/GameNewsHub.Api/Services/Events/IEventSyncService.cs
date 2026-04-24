namespace GameNewsHub.Api.Services.Events;

public interface IEventSyncService
{
    public Task DiscoverNewEventsAsync();
}