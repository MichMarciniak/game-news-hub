namespace GameNewsHub.Contracts;

public class ProfileDto
{
    public int Id { get; set; }
    public string Username { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public SettingsDto Settings { get; set; }

    public List<GameListItemDto> Games { get; set; }
    public List<GenreDto> Genres { get; set; }
    public List<EventListNameDto> Events { get; set; }
    public List<PlatformGroupDto> PlatformGroups { get; set; }
}

public class SettingsDto
{
    public bool ShowFutureRecommendations { get; set; }
}