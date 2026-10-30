using GameNewsHub.Api.Options;
using Microsoft.Extensions.Options;

namespace GameNewsHub.Api.Services;


public class CountFollowsBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CountFollowsBackgroundService> _logger;
    private readonly TimeSpan _period;

    public CountFollowsBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<CountFollowsBackgroundService> logger,
        IOptions<FollowServiceOptions> options)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _period = options.Value.Timespan;
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
                    var job = scope.ServiceProvider.GetRequiredService<FollowRecalculationJob>();
                    await job.RecalculateAsync();
                }
                
                _logger.LogInformation("Follows recalculated.");
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
        
    }
    
}