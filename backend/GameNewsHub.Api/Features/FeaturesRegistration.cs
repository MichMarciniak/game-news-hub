using GameNewsHub.Api.Features.Games;
using GameNewsHub.Api.Features.Genres;
using GameNewsHub.Api.Features.Recommendations;
using GameNewsHub.Api.Features.UserInterest;

namespace GameNewsHub.Api.Features;

public static class FeaturesRegistration
{
    public static IServiceCollection AddFeatureServices(this IServiceCollection services)
    {
        services.AddGameServices();
        services.AddGenreServices();
        services.AddInterestServices();
        services.AddRecommendationServices();
        return services;
    }
}