namespace GameNewsHub.Api.Entities;

public class UserGenreWeight
{
    public int UserId { get; set; }
    public int GenreId { get; set; }
    public double Weight { get; set; } = 0;
}