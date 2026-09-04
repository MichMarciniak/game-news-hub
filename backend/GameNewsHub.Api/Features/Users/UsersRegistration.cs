namespace GameNewsHub.Api.Features.Users;

public static class UsersRegistration
{
    public static IServiceCollection AddUsersServices(this IServiceCollection services)
    {
        services.AddScoped<UsersService>();

        return services;
    }
}