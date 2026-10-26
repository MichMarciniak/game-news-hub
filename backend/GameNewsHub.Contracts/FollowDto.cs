namespace GameNewsHub.Contracts;

public class BatchFollowDto<T>
{
    public List<T> Ids { get; set; }
}