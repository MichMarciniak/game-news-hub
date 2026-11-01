using GameNewsHub.Api.BackgroundServices.FollowCounter;
using GameNewsHub.Api.Options;
using Microsoft.Extensions.Options;

namespace GameNewsHub.Api.Services.FollowSnapshot;

public class SnapshotBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SnapshotBackgroundService> _logger;
    private readonly TimeSpan _period;

    public SnapshotBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<SnapshotBackgroundService> logger,
        IOptions<FollowServiceOptions> followServiceOptions)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _period = followServiceOptions.Value.SnapTimespan;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(_period);

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var job = scope.ServiceProvider.GetRequiredService<SnapshotFollowCount>();
                    await job.SnapshotFollowerCount();
                }
                _logger.LogInformation("Snapshot made");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}