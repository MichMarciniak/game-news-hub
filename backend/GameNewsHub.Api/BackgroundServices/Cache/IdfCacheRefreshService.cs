using System.ComponentModel;
using GameNewsHub.Recommendation;

namespace GameNewsHub.Api.Services.Cache;

public class IdfCacheRefreshService : BackgroundService 
{
    private static readonly TimeSpan RefreshInterval = TimeSpan.FromHours(1);

    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<IdfCacheRefreshService> _logger;


    public IdfCacheRefreshService(IServiceScopeFactory serviceScopeFactory, ILogger<IdfCacheRefreshService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        using var timer = new PeriodicTimer(RefreshInterval);

        do
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var idfCache = scope.ServiceProvider.GetRequiredService<IdfCacheService>();

                await idfCache.RefreshCacheAsync(ct);

            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "IDF cache refresh failed");
            }
        } while (await timer.WaitForNextTickAsync(ct));
    }
}