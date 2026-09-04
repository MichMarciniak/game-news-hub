using Data.Entities;

namespace GameNewsHub.Data.Entities;

public class Game
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    
    public string? CoverUrl { get; set; }

    public ICollection<Genre> Genres { get; set; } = new List<Genre>();
    public ICollection<Platform> Platforms { get; set; } = new List<Platform>();

    public GameType Type { get; set; } = GameType.MainGame;
    public int? ParentGameIgdbId { get; set; }
    public int? ParentGameId { get; set; }
    public Game? ParentGame { get; set; }
    public ICollection<Game> ChildGames { get; set; } = new List<Game>();

    public int IgdbId { get; set; }
}