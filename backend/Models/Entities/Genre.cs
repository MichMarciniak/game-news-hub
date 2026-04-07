namespace backend.Models.Entities;

public class Genre
{
    public int Id { get; set; }
    public string Name { get; set; }

    public int IgdbId { get; set; }

    public ICollection<Game> Games { get; set; } = new List<Game>();
}