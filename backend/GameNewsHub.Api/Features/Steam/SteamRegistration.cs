using GameNewsHub.Api.Features.Steam.Auth;

namespace GameNewsHub.Api.Features.Steam;

public static class SteamRegistration
{
    public static IServiceCollection AddSteamServices(this IServiceCollection services)
    {
        services.AddScoped<SteamService>();
        services.AddScoped<SteamAuthService>();

        return services;
    }
}