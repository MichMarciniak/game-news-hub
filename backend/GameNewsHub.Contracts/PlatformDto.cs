namespace GameNewsHub.Contracts;

public record PlatformDto
{
    public string Name { get; set; } = string.Empty;
    public int Id { get; set; }
}