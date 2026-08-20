using backend.Configuration;
using GameNewsHub.Sync.External;

namespace GameNewsHub.Sync;

public static class IgdbServiceExtension
{
    
    public static IServiceCollection AddIgdbServices(this IServiceCollection services, IConfiguration config)
    {
        var igdbOptions = config.GetSection("Api").Get<ApiConfig>();

        services.AddSingleton<IgdbAuthService>();
        
        services.AddHttpClient<IIgdbClient, IgdbClient>(client =>
        {
            client.BaseAddress = new Uri(igdbOptions.BaseUrl);
            client.DefaultRequestHeaders.Add("Client-ID", igdbOptions.ClientId);
        });

        return services;
    }
}