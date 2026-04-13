namespace backend.Models.Entities;

public class Game
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    
    public string? CoverUrl { get; set; }

    public ICollection<Genre> Genres { get; set; } = new List<Genre>();
    public ICollection<Platform> Platforms { get; set; } = new List<Platform>();

    public int IgdbId { get; set; }
}