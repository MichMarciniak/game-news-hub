using GameNewsHub.Sync.Sync.Events;

namespace GameNewsHub.Sync.Sync.Workers;

public class EventHydrationWorker : BackgroundService 
{
    private readonly ILogger<EventHydrationWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(2);
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public EventHydrationWorker(ILogger<EventHydrationWorker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(10000);
        _logger.LogInformation("EventHydrationWorker is starting.");
        while (!stoppingToken.IsCancellationRequested)
        {
            await _semaphore.WaitAsync(stoppingToken);
            try
            {
                _logger.LogInformation($"Starting event hydration at: {DateTimeOffset.Now}");

                using (var scope = _serviceProvider.CreateScope())
                {
                    var service = scope.ServiceProvider.GetRequiredService<IEventSyncService>();

                    await service.HydrateEventsAsync();

                }

                _logger.LogInformation("Hydration completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured during event hydration.");
            }
            finally
            {
                _semaphore.Release();
            }
            await Task.Delay(_checkInterval, stoppingToken);
        }
        _logger.LogInformation("EventHydrationWorker is stopping.");

    }
}