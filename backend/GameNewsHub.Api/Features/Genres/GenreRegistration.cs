namespace GameNewsHub.Api.Features.Genres;

public static class GenreRegistration
{
    public static IServiceCollection AddGenreServices(this IServiceCollection services)
    {
        services.AddScoped<IGenreService, GenreService>();
        return services;
    }
}