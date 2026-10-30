using GameNewsHub.Contracts.Shared;

namespace GameNewsHub.Contracts.Games;


public enum GameSortBy
{
    Name,
    Popularity,
    Relevance
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
