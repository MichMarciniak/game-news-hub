using GameNewsHub.Api.Features.Auth;
using GameNewsHub.Api.Features.Events;
using GameNewsHub.Api.Features.Games;
using GameNewsHub.Api.Features.Genres;
using GameNewsHub.Api.Features.Platforms;
using GameNewsHub.Api.Features.Recommendations;
using GameNewsHub.Api.Features.Users;

namespace GameNewsHub.Api.Features;

public static class FeaturesRegistration
{
    public static IServiceCollection AddFeatureServices(this IServiceCollection services)
    {
        services.AddGameServices();
        services.AddGenreServices();
        services.AddRecommendationServices();
        services.AddUsersServices();
        services.AddEventServices();
        services.AddPlatformServices();
        services.AddAuthServices();
        return services;
    }
}