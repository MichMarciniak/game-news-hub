using System.Text.Json.Serialization;

namespace GameNewsHub.External.Igdb.Dtos;

public record IgdbGameDto
{
    [JsonPropertyName("id")] public int IgdbId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public List<IgdbGenreResponse>? Genres { get; set; }
    public List<IgdbPlatformResponse>? Platforms { get; set; }
    [JsonPropertyName("game_type")] public IgdbGameTypeDto? GameType { get; set; }
    [JsonPropertyName("parent_game")] public int? ParentGame { get; set; }
    [JsonPropertyName("release_dates")] public List<ReleaseDate>? ReleaseDates { get; set; }
    [JsonPropertyName("external_games")] public List<ExternalGame>? ExternalGames { get; set; }
    public IgdbCoverDto? Cover { get; set; }
}

public class ExternalIdToIgdbIdDto 
{
    [JsonPropertyName("game")] public int IgdbId { get; set; }
    [JsonPropertyName("uid")] public string Uid { get; set; } = string.Empty;
}

public class GameIdContainer
{
    [JsonPropertyName("id")] public int IgdbId { get; set; }
}


public class IgdbGameTypeDto
{
    [JsonPropertyName("id")] public int IgdbId { get; set; }

    public string Type { get; set; } = string.Empty;
}

public class ReleaseDate
{
    public long Date { get; set; }
}

public class ExternalGame 
{
    [JsonPropertyName("uid")] public string Uid { get; set; }

    [JsonPropertyName("external_game_source")]
    public ExternalGameSource Source { get; set; }
}

public enum ExternalGameSource
{
    Steam = 1,
}