using Microsoft.AspNetCore.Identity;

namespace GameNewsHub.Data.Entities;

public class AppUser : IdentityUser<int>
{
    public DateTimeOffset CreatedAt { get; set; }

    public bool ShowFutureRecommendations { get; set; } = true;
    //public bool NotificationsEnabled { get; set; } = false;

    public ICollection<PlatformGroup> FollowedPlatformGroups { get; set; } = new List<PlatformGroup>();
    public ICollection<Game> FollowedGames { get; set; } = new List<Game>();
    public ICollection<Genre> FollowedGenres { get; set; } = new List<Genre>();

    public ICollection<Event> FollowedEvents { get; set; } = new List<Event>();
}