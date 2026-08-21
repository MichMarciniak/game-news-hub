using System.Text.Json.Serialization;

namespace GameNewsHub.Sync.Dtos;

public record IgdbPlatformResponse
{
    [JsonPropertyName("id")]
    public int IgdbId { get; set; }
    public string Name { get; set; } = string.Empty;
}