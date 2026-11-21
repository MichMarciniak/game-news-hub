using GameNewsHub.Api.Options;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.RateLimiting;

namespace GameNewsHub.Api.Configuration;

public static class RateLimiterConfiguration
{
    private static string GetPartitionKey(HttpContext ctx)
    {
        return ctx.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? ctx.Connection.RemoteIpAddress?.ToString()
            ?? "anonymous";
    }

    public static IServiceCollection AddRateLimiting(this IServiceCollection services, ApiRateLimitOptions apiRateLimitOptions)
    {
        services.AddRateLimiter(options =>
        {
            // Global default - per user, 60 req/min
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
                RateLimitPartition.GetFixedWindowLimiter(GetPartitionKey(ctx), _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 60,
                    Window = TimeSpan.FromMinutes(1),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0
                }));

            // Auth strict - 5 req/min for login, register, forgot/reset password, email confirmation
            options.AddPolicy("AuthStrict", ctx =>
                RateLimitPartition.GetFixedWindowLimiter(GetPartitionKey(ctx), _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 5,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0
                }));

            // Auth moderate - 10 req/min for change password, logout
            options.AddPolicy("AuthModerate", ctx =>
                RateLimitPartition.GetFixedWindowLimiter(GetPartitionKey(ctx), _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 10,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0
                }));

            // Steam auth - 10 req/min for Steam OpenID flow
            options.AddPolicy("SteamAuth", ctx =>
                RateLimitPartition.GetFixedWindowLimiter(GetPartitionKey(ctx), _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 10,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0
                }));

            // Steam API - 10 req/day for Steam Web API calls
            options.AddPolicy("SteamApi", ctx =>
                RateLimitPartition.GetFixedWindowLimiter(GetPartitionKey(ctx), _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = apiRateLimitOptions.Steam.PermitLimit,
                    Window = TimeSpan.FromDays(apiRateLimitOptions.Steam.WindowDays),
                    QueueLimit = 0
                }));

            // Write moderate - 30 req/min for follow/unfollow, wishlist, steam import
            options.AddPolicy("WriteModerate", ctx =>
                RateLimitPartition.GetFixedWindowLimiter(GetPartitionKey(ctx), _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 30,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 5
                }));

            // Read lenient - 100 req/min for events, games, recommendations, genres, platforms
            options.AddPolicy("ReadLenient", ctx =>
                RateLimitPartition.GetFixedWindowLimiter(GetPartitionKey(ctx), _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 100,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 20
                }));

            // Search lenient - 120 req/min for game search
            options.AddPolicy("SearchLenient", ctx =>
                RateLimitPartition.GetFixedWindowLimiter(GetPartitionKey(ctx), _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 120,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 20
                }));

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