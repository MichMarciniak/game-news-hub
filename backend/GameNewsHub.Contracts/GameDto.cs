using System.Text.Json.Serialization;

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

    public GameTypeString Type { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GameTypeString
{
    MainGame = 0,
    DlcAddon = 1,
    Expansion = 2,
    Bundle = 3,
    StandaloneExpansion = 4,
    Mod = 5,
    Episode = 6,
    Season = 7,
    Remake = 8,
    Remaster = 9,
    ExpandedGame = 10,
    Port = 11,
    Fork = 12,
    PackAddon = 13,
    Update = 14
}
