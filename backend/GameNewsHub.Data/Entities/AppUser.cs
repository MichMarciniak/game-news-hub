using Microsoft.AspNetCore.Identity;

namespace GameNewsHub.Api.Entities;

public class AppUser : IdentityUser<int>
{
    public DateTimeOffset CreatedAt { get; set; }
    
    public ICollection<Platform> FollowedPlatforms { get; set; } = new List<Platform>();
    public ICollection<Game> FollowedGames { get; set; } = new List<Game>();
    public ICollection<Genre> FollowedGenres { get; set; } = new List<Genre>();

    public ICollection<Event> FollowedEvents { get; set; } = new List<Event>();

}