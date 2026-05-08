using GameNewsHub.Api.Sync.Events;

namespace GameNewsHub.Api.Sync.Workers;

public class EventDiscoveryWorker : BackgroundService
{

    private readonly ILogger<EventDiscoveryWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _checkInterval = TimeSpan.FromDays(1);
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public EventDiscoveryWorker(ILogger<EventDiscoveryWorker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(1000);
        
        _logger.LogInformation("EventDiscoveryWorker is starting.");
        while (!stoppingToken.IsCancellationRequested)
        {
            await _semaphore.WaitAsync(stoppingToken);
            try
            {
                _logger.LogInformation($"Starting event discovery at: {DateTimeOffset.Now}");

                using (var scope = _serviceProvider.CreateScope())
                {
                    var service = scope.ServiceProvider.GetRequiredService<IEventSyncService>();

                    await service.DiscoverNewEventsAsync();

                }

                _logger.LogInformation("Discovery completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured during event discovery.");
            }
            finally
            {
                _semaphore.Release();
            }
            await Task.Delay(_checkInterval, stoppingToken);
        }
        _logger.LogInformation("EventDiscoveryWorker is stopping.");

    }
}