namespace GameNewsHub.Sync.Workers;

public static class WorkerServiceExtension
{
    public static IServiceCollection AddSyncBackgroundWorkers(this IServiceCollection services)
    {
        services.AddHostedService<EventDiscoveryWorker>();
        services.AddHostedService<EventHydrationWorker>();
        return services;
    }
}