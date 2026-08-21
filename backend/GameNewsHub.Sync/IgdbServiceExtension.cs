using backend.Configuration;
using GameNewsHub.Sync.External;

namespace GameNewsHub.Sync;

public static class IgdbServiceExtension
{
    
    public static IServiceCollection AddIgdbServices(this IServiceCollection services, IConfiguration config)
    {
        var igdbOptions = config.GetSection("Api").Get<ApiConfig>();

        services.AddSingleton<IgdbAuthService>();
        
        services.AddHttpClient("IgdbClientConfig", client =>
        {
            client.BaseAddress = new Uri(igdbOptions.BaseUrl);
            client.DefaultRequestHeaders.Add("Client-ID", igdbOptions.ClientId);
        });

        services.AddKeyedTransient<IIgdbClient, StandardIgdbClient>("Standard", (sp, _) =>
            new StandardIgdbClient(
                sp.GetRequiredService<IHttpClientFactory>().CreateClient("IgdbClientConfig"),
                sp.GetRequiredService<IgdbAuthService>(),
                sp.GetRequiredService<ILogger<StandardIgdbClient>>()
            ));
        
        services.AddKeyedTransient<IIgdbClient, BulkIgdbClient>("Bulk", (sp, _) =>
            new BulkIgdbClient(
                sp.GetRequiredService<IHttpClientFactory>().CreateClient("IgdbClientConfig"),
                sp.GetRequiredService<IgdbAuthService>(),
                sp.GetRequiredService<ILogger<BulkIgdbClient>>()
            ));

        var mode = config["IgdbMode"];
        var defaultKey = string.Equals(mode, "Bulk", StringComparison.OrdinalIgnoreCase) ? "Bulk" : "Standard";

        services.AddTransient<IIgdbClient>(sp =>
            sp.GetRequiredKeyedService<IIgdbClient>(defaultKey));

        return services;
    }
}