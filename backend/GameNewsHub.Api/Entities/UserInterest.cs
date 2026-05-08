namespace GameNewsHub.Api.Entities;

// system wagi
public class UserInterest
{
    public int UserId { get; set; }
    public AppUser User { get; set; }

    public int GenreId { get; set; }
    public Genre Genre { get; set; }

    public int Score { get; set; } = 0;
}