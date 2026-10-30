namespace GameNewsHub.Contracts.Shared;

public class SearchDto<T>
{
    public int Total { get; set; }
    public List<T> Items { get; set; } = [];
}
