using GameNewsHub.External.Igdb.Auth;
using GameNewsHub.External.Igdb.Client;
using GameNewsHub.External.Igdb.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GameNewsHub.External.Igdb;

public static class IgdbServiceExtensions
{ 
    public static IServiceCollection AddIgdbServices(
        this IServiceCollection services,
        IgdbOptions igdbOptions 
    )
    {
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

        var defaultKey = string.Equals(igdbOptions.Mode, "Bulk", StringComparison.OrdinalIgnoreCase) ? "Bulk" : "Standard";

        services.AddTransient<IIgdbClient>(sp =>
            sp.GetRequiredKeyedService<IIgdbClient>(defaultKey));

        return services;
    }
}