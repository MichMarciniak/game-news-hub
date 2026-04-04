namespace backend.Models.DTOs;

public record GenreDto
{
    public string Name { get; set; } = string.Empty;
    public int Id { get; set; }
}