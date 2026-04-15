namespace backend.Models.Entities;

public class EventGenreWeight
{
    public int EventId { get; set; }
    public int GenreId { get; set; }
    public double Weight { get; set; }
}