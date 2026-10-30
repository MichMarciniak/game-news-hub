namespace GameNewsHub.Api.Services;

public static class BackgroundRegistration
{
    public static IServiceCollection AddBackgroundServices(this IServiceCollection services)
    {
        services.AddScoped<FollowRecalculationJob>();
        services.AddHostedService<CountFollowsBackgroundService>();

        return services;
    }
}