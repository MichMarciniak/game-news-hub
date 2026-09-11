namespace GameNewsHub.Api.Features.Platforms;

public static class PlatformRegistration
{
    public static IServiceCollection AddPlatformServices(this IServiceCollection services)
    {
        services.AddScoped<PlatformService>();

        return services;
    }
}