namespace GameNewsHub.Api.Features.UserInterest.Update;

public interface IWeightUpdateQueue
{
    ValueTask QueueUpdateAsync(int userId);
    ValueTask<int> DequeueAsync(CancellationToken ct);
}