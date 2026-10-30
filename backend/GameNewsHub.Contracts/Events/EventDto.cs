using GameNewsHub.Contracts.Games;

namespace GameNewsHub.Contracts.Events;

public class EventDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset? EndTime { get; set; }
    public List<GameListItemDto> Games { get; set; } = [];
    public List<EventListNameDto> RelatedEvents { get; set; } = [];
    public int FollowerCount { get; set; }
    public string LiveStreamUrl { get; set; } = string.Empty;
}

public class EventListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int FollowerCount { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset? EndTime { get; set; }
}

public class EventListNameDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
