using GameNewsHub.External.Steam.Client;
using Microsoft.Extensions.DependencyInjection;

namespace GameNewsHub.External.Steam;

public static class SteamServiceExtension
{

    public static IServiceCollection AddSteamServices(this IServiceCollection services)
    {
        var url = "https://api.steampowered.com/";
        services.AddTransient<HttpClient>();
        services.AddHttpClient<SteamClient>(client =>
        {
            client.BaseAddress = new Uri(url);
        });

        return services;
    }
}