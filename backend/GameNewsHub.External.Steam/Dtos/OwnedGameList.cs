using System.Text.Json.Serialization;

namespace GameNewsHub.External.Steam.Dtos;

public class OwnedGameList
{
    [JsonPropertyName("game_count")] public int GameCount { get; set; }
    [JsonPropertyName("games")] public List<OwnedGame> Games { get; set; }
}