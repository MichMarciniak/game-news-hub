using Microsoft.AspNetCore.Identity;

namespace GameNewsHub.Api.Entities;

public class AppUser : IdentityUser<int>
{
    public DateTimeOffset CreatedAt { get; set; }

    public ICollection<UserInterest> Interests { get; set; }
    public ICollection<Platform> Platforms { get; set; }
    public ICollection<Game> FollowedGames { get; set; }
    
}