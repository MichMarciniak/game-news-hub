using GameNewsHub.Api.Features.UserInterest.Calculator;

namespace GameNewsHub.Api.Features.UserInterest.Update;

public class WeightUpdateWorker : BackgroundService
{
    private readonly IWeightUpdateQueue _queue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<WeightUpdateWorker> _logger;

    public WeightUpdateWorker(IWeightUpdateQueue queue, IServiceScopeFactory scopeFactory,
        ILogger<WeightUpdateWorker> logger)
    {
        _queue = queue;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Weight Update Worker starting");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var userId = await _queue.DequeueAsync(stoppingToken);

                using var scope = _scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IUserWeightService>();

                await service.UpdateUserWeightsAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing weight update");   
            }
        }
    }
}