using System.ComponentModel;
using GameNewsHub.Api.Services.Events;

namespace GameNewsHub.Api.Services.Background;

public class EventHydrationWorker : BackgroundService 
{
    private readonly ILogger<EventHydrationWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(2);

    public EventHydrationWorker(ILogger<EventHydrationWorker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("EventHydrationWorker is starting.");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation($"Starting event hydration at: {DateTimeOffset.Now}");

                using (var scope = _serviceProvider.CreateScope())
                {
                    var service = scope.ServiceProvider.GetRequiredService<IEventSyncService>();

                    await service.DiscoverNewEventsAsync();

                }

                _logger.LogInformation("Hydration completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured during event hydration.");
            }
            await Task.Delay(_checkInterval, stoppingToken);
        }
        _logger.LogInformation("EventHydrationWorker is stopping.");

    }
}