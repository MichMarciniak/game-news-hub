namespace GameNewsHub.Contracts;

public class EventDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset? EndTime { get; set; }
    public List<GameListItemDto>? Games { get; set; }
    public List<EventListNameDto>? RelatedEvents { get; set; }
}

public class EventListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset? EndTime { get; set; }
}

public class EventListNameDto
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class NormalizedEventListItemDto 
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string NormalizedName { get; set; }
}
