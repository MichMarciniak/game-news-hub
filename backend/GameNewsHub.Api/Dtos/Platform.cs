namespace GameNewsHub.Api.Dtos;


public record PlatformResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}