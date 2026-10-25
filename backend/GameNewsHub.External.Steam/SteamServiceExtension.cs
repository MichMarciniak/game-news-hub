using GameNewsHub.External.Steam.Client;
using GameNewsHub.External.Steam.Options;
using GameNewsHub.Hosting.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GameNewsHub.External.Steam;

public static class SteamServiceExtension
{

    public static IServiceCollection AddSteamServices(
        this IServiceCollection services,
        IConfiguration configuration
        )
    {
        var steamOptions = services.AddValidatedOptions<SteamOptions>(configuration);
        
        
        services.AddHttpClient<SteamApiClient>(client =>
        {
            client.BaseAddress = new Uri(steamOptions.ApiUrl);
        });
        services.AddHttpClient<SteamAuthClient>(client =>
        {
            client.BaseAddress = new Uri(steamOptions.AuthUrl);
        });

        return services;
    }
}