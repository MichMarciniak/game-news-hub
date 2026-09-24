using System.Text.Json.Serialization;

namespace GameNewsHub.Sync.Dtos;

public record IgdbGameResponse
{
    [JsonPropertyName("id")] public int IgdbId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;

    public List<IgdbGenreResponse>? Genres { get; set; }
    public List<IgdbPlatformResponse>? Platforms { get; set; }

    [JsonPropertyName("game_type")] public IgdbGameTypeDto? GameType { get; set; }

    [JsonPropertyName("parent_game")] public int? ParentGame { get; set; }
    
    [JsonPropertyName("release_dates")] public List<ReleaseDate>? ReleaseDates { get; set; }

    public IgdbCoverResponse? Cover { get; set; }
}

public record GameIdContainer(int Id);

public class IgdbGameTypeDto
{
    [JsonPropertyName("id")] public int IgdbId { get; set; }

    public string Type { get; set; } = string.Empty;
}

public class ReleaseDate
{
    public long Date { get; set; }
}