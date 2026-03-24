namespace backend.Models.Entities;

public class Game
{
    public int? Id { get; set; }
    public string Title { get; set; }
    public string Summary { get; set; }
    
    public string? CoverUrl { get; set; }

    public ICollection<Genre> Genres { get; set; } = new List<Genre>();
    public ICollection<Platform> Platforms { get; set; } = new List<Platform>();

}