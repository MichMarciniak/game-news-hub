using Microsoft.AspNetCore.Identity;

namespace backend.Models.Entities;

public class AppUser : IdentityUser<int>
{
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<UserInterest> Interests { get; set; }
    public ICollection<Platform> Platforms { get; set; }
    public ICollection<Game> FollowedGames { get; set; }
    
}