namespace GameNewsHub.Api.Features.Events;

public static class EventRegistraion
{
    public static IServiceCollection AddEventServices(this IServiceCollection services)
    {
        services.AddScoped<EventService>();
        services.AddScoped<FollowEventService>();

        return services;
    }
}