using GameNewsHub.Contracts.Events;
using GameNewsHub.Contracts.Games;
using GameNewsHub.Contracts.Genres;
using GameNewsHub.Contracts.Platforms;

namespace GameNewsHub.Contracts.User;

public class ProfileDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public SettingsDto Settings { get; set; }
    public bool IsAdmin { get; set; }  
    public string? SteamId { get; set; }

}

public class FullProfileDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public SettingsDto Settings { get; set; }

    public List<GameListItemDto> Games { get; set; } = [];
    public List<GenreDto> Genres { get; set; } = [];
    public List<EventListNameDto> Events { get; set; } = [];
    public List<PlatformGroupDto> PlatformGroups { get; set; } = [];
    public string? SteamId { get; set; }
}