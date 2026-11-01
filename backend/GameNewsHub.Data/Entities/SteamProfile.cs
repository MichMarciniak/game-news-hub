namespace GameNewsHub.Data.Entities;

public class SteamProfile
{
    public int Id { get; set; }
    public ulong SteamId { get; set; }
    public int AppUserId { get; set; }
    public AppUser AppUser { get; set; }
    public ICollection<ulong> FriendSteamIds { get; set; } = [];

    public DateTimeOffset LastImportedAt { get; set; }
}