namespace GameNewsHub.Data.Entities;

public class EventFollowerSnapshot
{
    public long Id { get; set; }
    public int EventId { get; set; }
    public Event Event { get; set; }

    public int FollowerCount { get; set; }
    public DateTimeOffset RecordedAt { get; set; }
}