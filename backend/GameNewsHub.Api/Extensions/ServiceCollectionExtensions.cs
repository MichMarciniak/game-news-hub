using System.Text;
using backend.Configuration;
using GameNewsHub.Api.External;
using GameNewsHub.Api.Features.Genres;
using GameNewsHub.Api.Features.Games;
using GameNewsHub.Api.Sync.Events;
using GameNewsHub.Api.Sync.Games;
using GameNewsHub.Api.Sync.Workers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

namespace backend.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthDocumentation(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancelToken) =>
            {
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        
                var scheme = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                };

                document.Components.SecuritySchemes.Add("Bearer", scheme);

                return Task.CompletedTask;
            });
        });
        return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
    {
        var key = Encoding.ASCII.GetBytes(config["Jwt:Key"]);

        services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            });

        return services;
    }

    public static IServiceCollection AddIgdbServices(this IServiceCollection services, IConfiguration config)
    {
        var igdbOptions = config.GetSection("Api").Get<ApiConfig>();

        services.AddSingleton<IgdbAuthService>();
        
        services.AddHttpClient<IIgdbClient, IgdbClient>(client =>
        {
            client.BaseAddress = new Uri(igdbOptions.BaseUrl);
            client.DefaultRequestHeaders.Add("Client-ID", igdbOptions.ClientId);
        });

        return services;
    }
}