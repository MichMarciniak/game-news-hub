using GameNewsHub.External.Igdb.Auth;
using GameNewsHub.External.Igdb.Client;
using GameNewsHub.External.Igdb.Options;
using Microsoft.Extensions.DependencyInjection;

namespace GameNewsHub.External.Igdb;

public static class IgdbServiceExtensions
{ 
    public static IServiceCollection AddIgdbServices(
        this IServiceCollection services,
        IgdbOptions igdbOptions 
    )
    {
        services.AddSingleton<IgdbAuthService>();

        services.AddHttpClient<IIgdbClient, IgdbClient>(client =>
        {
            client.BaseAddress = new Uri(igdbOptions.BaseUrl);
            client.DefaultRequestHeaders.Add("Client-ID", igdbOptions.ClientId);
        });

        return services;
    }
}