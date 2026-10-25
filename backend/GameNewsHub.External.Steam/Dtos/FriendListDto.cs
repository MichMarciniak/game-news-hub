using System.Text.Json.Serialization;

namespace GameNewsHub.External.Steam.Dtos;

public record FriendListResponse 
{
    [JsonPropertyName("friendslist")]
    public FriendListDto FriendsList { get; init; }
}

public record FriendListDto
{
    [JsonPropertyName("friends")]
    public List<FriendDto> Friends { get; init; }
}

public record FriendDto
{
    [JsonPropertyName("steamid")]
    public string SteamId { get; init; }
}

