using GameNewsHub.Sync.Core.Events;
using GameNewsHub.Sync.Options;
using Microsoft.Extensions.Options;

namespace GameNewsHub.Sync.Workers;

public class EventHydrationWorker : BackgroundService
{
    private static readonly SemaphoreSlim Semaphore = new(1, 1);
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(2);
    private readonly ILogger<EventHydrationWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly int _days;

    public EventHydrationWorker(
        ILogger<EventHydrationWorker> logger,
        IServiceProvider serviceProvider,
        IOptions<SyncRangeOptions> syncOptions
        )
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _days = syncOptions.Value.Days;

    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(10000);
        
        _logger.LogInformation("EventHydrationWorker is starting.");
        while (!stoppingToken.IsCancellationRequested)
        {
            await Semaphore.WaitAsync(stoppingToken);
            try
            {
                _logger.LogInformation($"Starting event hydration at: {DateTimeOffset.Now}");

                using (var scope = _serviceProvider.CreateScope())
                {
                    var service = scope.ServiceProvider.GetRequiredService<EventHydrationService>();
                    
                    await service.HydratePendingEventsAsync(_days);
                }

                _logger.LogInformation("Hydration completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured during event hydration.");
            }
            finally
            {
                Semaphore.Release();
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("EventHydrationWorker is stopping.");
    }
}