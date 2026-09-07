namespace GameNewsHub.Contracts;


public class GameListItemDto 
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string? CoverUrl { get; set; }
}

public class GameDetailDto 
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string? CoverUrl { get; set; }
    public List<GenreDto> Genres { get; set; } = new List<GenreDto>();

    public List<PlatformGroupDto> PlatformGroups { get; set; } = new List<PlatformGroupDto>();

    public List<GameListItemDto>? Addons { get; set; }

    public int? ParentGameId { get; set; }
}
