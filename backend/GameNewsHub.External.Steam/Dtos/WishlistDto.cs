using System.Text.Json.Serialization;

namespace GameNewsHub.External.Steam.Dtos;

public record WishlistResponse 
{
    [JsonPropertyName("response")]
    public WishlistDto Response { get; init; }
}

public record WishlistDto
{
    [JsonPropertyName("items")]
    public List<WishlistItemDto> Items { get; init; }
}

public record WishlistItemDto
{
    [JsonPropertyName("appid")]
    public ulong AppId { get; init; }
    [JsonPropertyName("date_added")]
    public long DateAdded { get; init; }
}