namespace GameNewsHub.Api.Services.Games;

public interface IGameSyncService
{
    public Task SyncUpcomingGamesAsync(int limit);
}