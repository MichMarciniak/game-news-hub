namespace GameNewsHub.Api.Features.UserInterest;

public static class UserInterestRegistration
{
    public static IServiceCollection AddInterestServices(this IServiceCollection services)
    {
        services.AddScoped<IUserInterestService, UserInterestService>();
        return services;
    }
}