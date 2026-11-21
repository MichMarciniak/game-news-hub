using GameNewsHub.Api.Options;
using Microsoft.AspNetCore.RateLimiting;
using System.Text.Json;
using System.Threading.RateLimiting;

namespace GameNewsHub.Api.Configuration;

public static class RateLimiterConfiguration
{
    public static IServiceCollection AddRateLimiting(this IServiceCollection services, ApiRateLimitOptions apiRateLimitOptions)
    {
        services.AddRateLimiter(options =>
        {
            // Global default
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
                RateLimitPartition.GetFixedWindowLimiter("default", _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 60,
                    Window = TimeSpan.FromMinutes(1),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0
                }));

            // Auth strict - 5 req/min for login, register, forgot/reset password, email confirmation
            options.AddFixedWindowLimiter("AuthStrict", o =>
            {
                o.PermitLimit = 5;
                o.Window = TimeSpan.FromMinutes(1);
                o.QueueLimit = 0;
            });

            // Auth moderate - 10 req/min for change password, logout
            options.AddFixedWindowLimiter("AuthModerate", o =>
            {
                o.PermitLimit = 10;
                o.Window = TimeSpan.FromMinutes(1);
                o.QueueLimit = 0;
            });

            // Steam auth - 10 req/min for Steam OpenID flow
            options.AddFixedWindowLimiter("SteamAuth", o =>
            {
                o.PermitLimit = 10;
                o.Window = TimeSpan.FromMinutes(1);
                o.QueueLimit = 0;
            });

            // Steam API - 10 req/day for Steam Web API calls
            options.AddFixedWindowLimiter("SteamApi", o =>
            {
                o.PermitLimit = apiRateLimitOptions.Steam.PermitLimit;
                o.Window = TimeSpan.FromDays(apiRateLimitOptions.Steam.WindowDays);
                o.QueueLimit = 0;
            });

            // Write moderate - 30 req/min for follow/unfollow, wishlist, steam import
            options.AddFixedWindowLimiter("WriteModerate", o =>
            {
                o.PermitLimit = 30;
                o.Window = TimeSpan.FromMinutes(1);
                o.QueueLimit = 5;
            });

            // Read lenient - 100 req/min for events, games, recommendations, genres, platforms
            options.AddFixedWindowLimiter("ReadLenient", o =>
            {
                o.PermitLimit = 100;
                o.Window = TimeSpan.FromMinutes(1);
                o.QueueLimit = 20;
            });

            // Search lenient - 120 req/min for game search
            options.AddFixedWindowLimiter("SearchLenient", o =>
            {
                o.PermitLimit = 120;
                o.Window = TimeSpan.FromMinutes(1);
                o.QueueLimit = 20;
            });

            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            
            // Custom response when rate limit is exceeded
            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.Response.ContentType = "application/json";
                
                var retryAfter = GetRetryAfter(context);
                if (retryAfter != "unknown")
                {
                    context.HttpContext.Response.Headers.Append("Retry-After", retryAfter);
                }
                
                var response = new
                {
                    error = "Rate limit exceeded",
                    message = "Too many requests. Please try again later.",
                    retryAfter = retryAfter
                };
                
                var json = JsonSerializer.Serialize(response);
                await context.HttpContext.Response.WriteAsync(json, token);
            };
        });

        return services;
    }
    
    private static string GetRetryAfter(OnRejectedContext context)
    {
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            return retryAfter.ToString();
        }
        return "unknown";
    }
}