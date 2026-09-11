namespace GameNewsHub.Api.Features.Auth;

public static class AuthRegistration
{
    public static IServiceCollection AddAuthServices(this IServiceCollection services)
    {
        services.AddScoped<AuthService>();
        services.AddScoped<TokenService>();

        return services;
    }
}