using System.Security.Claims;
using System.Text;
using GameNewsHub.Api.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

namespace GameNewsHub.Api.Extensions;

public static class AuthExtension
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
                    In = ParameterLocation.Header
                };

                document.Components.SecuritySchemes.Add("Bearer", scheme);

                return Task.CompletedTask;
            });

            options.AddOperationTransformer((operation, context, cancelToken) =>
            {
                var requiresAuthorization = context.Description.ActionDescriptor.EndpointMetadata
                    .OfType<IAuthorizeData>()
                    .Any();

                if (requiresAuthorization)
                {
                    operation.Security ??= [];
                    operation.Security.Add(new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference("Bearer", context.Document)] = []
                    });
                }

                return Task.CompletedTask;
            });
        });
        return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, JwtTokenOptions options)
    {
        var key = Encoding.ASCII.GetBytes(options.SigningKey);

        if (key.Length == 0) throw new Exception("Token:SigningKey is required");

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
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = options.Issuer,
                    ValidAudience = options.Audience,
                    ClockSkew = TimeSpan.FromSeconds(30)
                };

                opt.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var tokenType = context.Principal?.FindFirstValue("token_type");
                        if (tokenType != "access") context.Fail("Invalid token type");

                        return Task.CompletedTask;
                    }
                };
            });

        return services;
    }
}