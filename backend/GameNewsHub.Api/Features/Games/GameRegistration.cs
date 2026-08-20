namespace GameNewsHub.Api.Features.Games;

public static class GameRegistration
{
    public static IServiceCollection AddGameServices(this IServiceCollection services)
    {
        services.AddScoped<GameService>();
        return services;
    }
}