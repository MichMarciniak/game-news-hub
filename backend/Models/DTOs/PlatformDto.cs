namespace backend.Models.DTOs;

public record PlatformDto
{
    public string Name { get; set; } = string.Empty;
    public int Id { get; set; }
}