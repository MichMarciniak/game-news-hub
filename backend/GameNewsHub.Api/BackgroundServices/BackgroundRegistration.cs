using GameNewsHub.Api.BackgroundServices.FollowCounter;
using GameNewsHub.Api.Services.FollowSnapshot;

namespace GameNewsHub.Api.Services;

public static class BackgroundRegistration
{
    public static IServiceCollection AddBackgroundServices(this IServiceCollection services)
    {
        services.AddScoped<FollowRecalculationJob>();
        services.AddHostedService<CountFollowsBackgroundService>();

        services.AddScoped<SnapshotFollowCount>();
        services.AddHostedService<SnapshotBackgroundService>();
        return services;
    }
}