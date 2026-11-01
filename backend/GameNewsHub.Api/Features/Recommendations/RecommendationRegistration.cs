namespace GameNewsHub.Api.Features.Recommendations;

public static class RecommendationRegistration
{
    public static IServiceCollection AddRecommendationServices(this IServiceCollection services)
    {
        services.AddScoped<RecMapper>();
        services.AddScoped<RecommendationService>();

        return services;
    }
}