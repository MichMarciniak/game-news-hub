namespace GameNewsHub.Contracts;

public record GenreDto
{
    public string Name { get; set; } = string.Empty;
    public int Id { get; set; }
}