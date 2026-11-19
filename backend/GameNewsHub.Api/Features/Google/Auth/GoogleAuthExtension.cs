using GameNewsHub.Api.Options;
using GameNewsHub.Hosting.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GameNewsHub.Api.Features.Google.Auth;

public static class GoogleAuthExtension
{
    public static IServiceCollection AddGoogleAuth(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var googleOptions = services.AddValidatedOptions<GoogleAuthOptions>(configuration);

        services.AddAuthentication()
            .AddGoogle(options =>
            {
                options.ClientId = googleOptions.ClientId;
                options.ClientSecret = googleOptions.ClientSecret;
                options.CallbackPath = "/signin-google";
                options.Scope.Add("openid");
                options.Scope.Add("email");
                options.Scope.Add("profile");

                options.CorrelationCookie.SameSite = SameSiteMode.Lax;
                options.CorrelationCookie.IsEssential = true;
                options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

                options.SignInScheme = IdentityConstants.ExternalScheme;
            });

        services.AddAuthorization();

        services.AddScoped<GoogleAuthService>();

        return services;
    }
}
