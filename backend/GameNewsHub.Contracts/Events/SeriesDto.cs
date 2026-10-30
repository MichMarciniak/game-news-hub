namespace GameNewsHub.Contracts.Events;

public class SeriesDto
{
    public int Id { get; set; }
}

public class AdminSeriesDto
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public List<EventListItemDto> Events { get; set; } = [];
}
