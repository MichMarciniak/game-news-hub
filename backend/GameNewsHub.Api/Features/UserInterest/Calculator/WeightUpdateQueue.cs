using System.Threading.Channels;

namespace GameNewsHub.Api.Features.UserInterest.Update;

public class WeightUpdateQueue : IWeightUpdateQueue
{
    private readonly Channel<int> _queue = Channel.CreateBounded<int>(1000);
    
    public async ValueTask QueueUpdateAsync(int userId)
    {
        await _queue.Writer.WriteAsync(userId);
    }

    public async ValueTask<int> DequeueAsync(CancellationToken ct)
    {
        return await _queue.Reader.ReadAsync(ct);
    }
}