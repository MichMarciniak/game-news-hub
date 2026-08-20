namespace GameNewsHub.Sync.Dtos;

public record IgdbPlatformResponse
{
    public string Name { get; set; } = string.Empty;
    public int Id { get; set; }
}