namespace GameNewsHub.Contracts;

public class EventDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public long StartTime { get; set; }
    public long? EndTime { get; set; }
    
    public List<GameListItemDto>? Games { get; set; }
}

public class EventListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public long StartTime { get; set; }
    public long? EndTime { get; set; }
}