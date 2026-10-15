using System.Text.Json.Serialization;

namespace GameNewsHub.External.Steam.Dtos;

public class OwnedGame
{
    [JsonPropertyName("appid")] public int AppId { get; set; }
    [JsonPropertyName("playtime_forever")] public long PlaytimeForeverMinutes { get; set; }
    [JsonPropertyName("rtime_last_played")] public long RtimeLastPlayedUnixSeconds { get; set; }
    
}