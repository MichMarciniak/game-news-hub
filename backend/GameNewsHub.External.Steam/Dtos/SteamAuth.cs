namespace GameNewsHub.External.Steam.Dtos;

public record SteamAuthResult
{
    public bool IsSuccess { get; set; }
    public string? SteamId { get; set; }
    public string? ErrorMessage { get; set; }
}