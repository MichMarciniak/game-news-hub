using System.Text.Json.Serialization;

namespace GameNewsHub.Api.External;

public record TwitchToken
{
    [JsonPropertyName("access_token")] public string AccessToken { get; set; }

    [JsonPropertyName("expires_in")] public int ExpiresIn { get; set; }

    [JsonPropertyName("token_type")] public string TokenType { get; set; }
}