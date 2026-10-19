using GameNewsHub.Sync.Core.Events;
using GameNewsHub.Sync.Core.Events.Calculators;
using GameNewsHub.Sync.Core.Games;
using GameNewsHub.Sync.Core.Genres;
using GameNewsHub.Sync.Core.Platforms;
using Microsoft.Extensions.DependencyInjection;

namespace GameNewsHub.Sync.Core;

public static class SyncCoreServiceExtensions
{
    public static IServiceCollection AddSyncCoreServices(this IServiceCollection services)
    {
        services.AddScoped<IEventWeightCalculator, EventWeightCalculator>();
        services.AddScoped<EventDiscoveryService>();
        services.AddScoped<EventHydrationService>();
        services.AddGameSyncServices();
        services.AddScoped<IGenreSyncService, GenreSyncService>();
        services.AddScoped<IPlatformSyncService, PlatformSyncService>();

        return services;
    }
}