using GameNewsHub.Sync.Core.Events;
using GameNewsHub.Sync.Options;
using Microsoft.Extensions.Options;

namespace GameNewsHub.Sync.Workers;

public class EventDiscoveryWorker : BackgroundService
{
    private static readonly SemaphoreSlim Semaphore = new(1, 1);
    private readonly TimeSpan _checkInterval = TimeSpan.FromDays(1);

    private readonly ILogger<EventDiscoveryWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly int _days;

    public EventDiscoveryWorker(
        ILogger<EventDiscoveryWorker> logger,
        IServiceProvider serviceProvider,
        IOptions<SyncRangeOptions> syncOptions)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _days = syncOptions.Value.Days;
        
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(1000);

        _logger.LogInformation("EventDiscoveryWorker is starting.");
        while (!stoppingToken.IsCancellationRequested)
        {
            await Semaphore.WaitAsync(stoppingToken);
            try
            {
                _logger.LogInformation($"Starting event discovery at: {DateTimeOffset.Now}");

                using (var scope = _serviceProvider.CreateScope())
                {
                    var service = scope.ServiceProvider.GetRequiredService<EventDiscoveryService>();

                    var from = DateTimeOffset.UtcNow.AddDays(-_days).ToUnixTimeSeconds();
                    var to = DateTimeOffset.UtcNow.AddDays(30).ToUnixTimeSeconds();

                    await service.DiscoverNewEventsAsync(from, to);
                }

                _logger.LogInformation("Discovery completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured during event discovery.");
            }
            finally
            {
                Semaphore.Release();
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("EventDiscoveryWorker is stopping.");
    }
}