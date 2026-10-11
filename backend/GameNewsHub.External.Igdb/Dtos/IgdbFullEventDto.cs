using System.Text.Json.Serialization;

namespace GameNewsHub.External.Igdb.Dtos;

public record IgdbFullEventDto 
{
    [JsonPropertyName("id")] public int IgdbId { get; set; }
    public string Name { get; set; }
    [JsonPropertyName("start_time")] public long StartTime { get; set; }
    [JsonPropertyName("end_time")] public long? EndTime { get; set; }
    public string? Description { get; set; } = string.Empty;
    [JsonPropertyName("live_stream_url")] public string? LiveStreamUrl { get; set; } = string.Empty;
    public List<GameIdContainer>? Games { get; set; }
}