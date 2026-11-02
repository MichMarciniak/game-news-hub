using GameNewsHub.Api.BackgroundServices.Cache;
using GameNewsHub.Api.BackgroundServices.FollowCounter;
using GameNewsHub.Api.BackgroundServices.FollowSnapshot;
using GameNewsHub.Api.Services.FollowSnapshot;

namespace GameNewsHub.Api.BackgroundServices;

public static class BackgroundRegistration
{
    public static IServiceCollection AddBackgroundServices(this IServiceCollection services)
    {
        services.AddScoped<FollowRecalculationJob>();
        services.AddHostedService<CountFollowsBackgroundService>();

        services.AddScoped<SnapshotFollowCount>();
        services.AddHostedService<SnapshotBackgroundService>();

        services.AddHostedService<IdfCacheRefreshService>();
        return services;
    }
}