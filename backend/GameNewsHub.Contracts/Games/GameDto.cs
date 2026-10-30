using GameNewsHub.Contracts.Genres;
using GameNewsHub.Contracts.Platforms;

namespace GameNewsHub.Contracts.Games;

public class GameListItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? CoverUrl { get; set; }
}

public class GameDetailDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string? CoverUrl { get; set; }
    public List<GenreDto> Genres { get; set; } = [];

    public List<PlatformGroupWithPlatformsDto> PlatformGroups { get; set; } = [];

    public List<GameListItemDto> Addons { get; set; } = [];
    public DateTimeOffset? ReleaseDate { get; set; }
    public ParentGameDto? ParentGame { get; set; }
    public GameTypeString Type { get; set; }
}

public class ParentGameDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
}