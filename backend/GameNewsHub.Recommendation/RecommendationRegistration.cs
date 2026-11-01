using Microsoft.Extensions.DependencyInjection;

namespace GameNewsHub.Recommendation;

public static class RecommendationRegistration
{
    public static IServiceCollection AddRecommendation(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddScoped<IdfCacheService>();
        services.AddScoped<UserProfileBuilder>();
        services.AddScoped<EventScoringEngine>();
        services.AddSingleton<EventVectorRepresenter>();
        return services;
    }
}