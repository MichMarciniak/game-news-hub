using GameNewsHub.Sync.Sync.Events;
using GameNewsHub.Sync.Sync.Games;
using GameNewsHub.Sync.Sync.Workers;

namespace GameNewsHub.Sync.Sync;

public static class SyncRegistration
{
    public static IServiceCollection AddSyncServices(this IServiceCollection services)
    {
        services.AddScoped<IEventWeightCalculator, EventWeightCalculator>();
        
        services.AddScoped<IEventSyncService, EventSyncService>();
        services.AddScoped<IGameSyncService, GameSyncService>();
        services.AddScoped<IGenreSyncService, GenreSyncService>();

        services.AddHostedService<EventDiscoveryWorker>();
        services.AddHostedService<EventHydrationWorker>();
        return services;
    }
}