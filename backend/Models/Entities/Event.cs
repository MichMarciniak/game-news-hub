using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Entities;

public class Event
{
    public int Id { get; set; } 
    public string Name { get; set; }
    
    public string Description { get; set; }
                                            
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset? EndTime { get; set; }

    public int IgdbId { get; set; }

    public ICollection<Game> Games { get; set; }

    // moze bedzie potrzebne pozniej
    // public boolean is_user_added {get; set;}
}