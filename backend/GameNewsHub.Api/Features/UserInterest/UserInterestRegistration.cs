using GameNewsHub.Api.Features.UserInterest.Calculator;
using GameNewsHub.Api.Features.UserInterest.Update;

namespace GameNewsHub.Api.Features.UserInterest;

public static class UserInterestRegistration
{
    public static IServiceCollection AddInterestServices(this IServiceCollection services)
    {
        services.AddScoped<IUserInterestService, UserInterestService>();

        services.AddScoped<UserGenreWeightCalculator>();
        services.AddScoped<IUserWeightService, UserWeightService>();
        services.AddHostedService<WeightUpdateWorker>();
        services.AddSingleton<IWeightUpdateQueue, WeightUpdateQueue>();
        return services;
    }
}