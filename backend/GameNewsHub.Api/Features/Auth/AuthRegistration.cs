using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace GameNewsHub.Api.Features.Auth;

public static class AuthRegistration
{

    public static IServiceCollection AddAuthServices(this IServiceCollection services)
    {
        services.AddScoped<AuthService>();
        services.AddScoped<TokenService>();
        services.AddScoped<IEmailSender, EmailSender>();

        return services;
    }
}