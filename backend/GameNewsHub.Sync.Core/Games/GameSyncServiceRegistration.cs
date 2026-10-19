using Microsoft.Extensions.DependencyInjection;

namespace GameNewsHub.Sync.Core.Games;

public static class GameSyncServiceRegistration
{
    public static IServiceCollection AddGameSyncServices(this IServiceCollection services)
    {
        services.AddScoped<IGameSyncService, GameSyncService>();
        return services;
    }
}