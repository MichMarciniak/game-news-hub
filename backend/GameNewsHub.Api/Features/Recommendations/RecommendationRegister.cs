namespace GameNewsHub.Api.Features.Recommendations;

public static class RecommendationRegister
{
    public static IServiceCollection AddRecommendationServices(this IServiceCollection service)
    {
        service.AddScoped< RecommendationService>();
        return service;
    }
}