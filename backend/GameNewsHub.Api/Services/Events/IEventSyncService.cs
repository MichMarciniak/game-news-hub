namespace GameNewsHub.Api.Services.Events;

public interface IEventSyncService
{
    public Task SyncUpcomingGamesAsync(int days);
}