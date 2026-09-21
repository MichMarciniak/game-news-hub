using System.Text.Json.Serialization;

namespace GameNewsHub.Contracts;

public class SearchDto<T>
{
    public int Total { get; set; }
    public List<T> Items { get; set; }
}

public class GetGameSearchParams
{
    public string? Query { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 24;
    public GameSortBy SortBy { get; set; } = GameSortBy.Relevance;
    public bool Descending { get; set; } = false;
    public List<int>? GenreIds { get; set; }
    public List<int>? PlatformIds { get; set; }

}