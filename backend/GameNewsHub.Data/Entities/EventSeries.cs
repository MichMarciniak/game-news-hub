using GameNewsHub.Data.Entities;

namespace Data.Entities;

public class EventSeries
{
    public int Id { get; set; }
    public ICollection<Event> Events { get; set; } = new List<Event>();
    
    
}