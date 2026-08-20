namespace GameNewsHub.Sync.Dtos;

public record IgdbGenreResponse 
{
    public string Name { get; set; } = string.Empty;
    public int Id { get; set; }
}