namespace GameNewsHub.Contracts.Shared;

public class BatchFollowDto<T>
{
    public List<T> Ids { get; set; } = [];
}