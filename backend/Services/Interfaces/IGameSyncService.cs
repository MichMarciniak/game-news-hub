namespace backend.Services.Interfaces;

public interface IGameSyncService
{
    public Task SyncUpcomingGamesAsync(int limit);
}