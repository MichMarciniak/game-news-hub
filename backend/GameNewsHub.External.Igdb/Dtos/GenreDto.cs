using System.Text.Json.Serialization;

namespace GameNewsHub.External.Igdb.Dtos;

public record IgdbGenreResponse
{
    [JsonPropertyName("id")] public int IgdbId { get; set; }

    public string Name { get; set; } = string.Empty;
}