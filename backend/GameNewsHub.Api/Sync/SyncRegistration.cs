using GameNewsHub.Api.Sync.Events;
using GameNewsHub.Api.Sync.Games;
using GameNewsHub.Api.Sync.Workers;

namespace GameNewsHub.Api.Sync;

public static class SyncRegistration
{
    public static IServiceCollection AddSyncServices(this IServiceCollection services)
    {
        services.AddScoped<IEventWeightCalculator, EventWeightCalculator>();
        
        services.AddScoped<IEventSyncService, EventSyncService>();
        services.AddScoped<IGameSyncService, GameSyncService>();

        services.AddHostedService<EventDiscoveryWorker>();
        services.AddHostedService<EventHydrationWorker>();
        return services;
    }
}