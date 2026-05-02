using System.ComponentModel;
using GameNewsHub.Api.Services.Events;

namespace GameNewsHub.Api.Services.Background;

public class EventDiscoveryWorker : BackgroundService
{

    private readonly ILogger<EventDiscoveryWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _checkInterval = TimeSpan.FromDays(1);

    public EventDiscoveryWorker(ILogger<EventDiscoveryWorker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("EventDiscoveryWorker is starting.");
        while (!stoppingToken.IsCancellationRequested)
        {
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
            await Task.Delay(_checkInterval, stoppingToken);
        }
        _logger.LogInformation("EventDiscoveryWorker is stopping.");

    }
}