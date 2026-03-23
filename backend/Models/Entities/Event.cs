using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Entities;

public class Event
{
    public int Id { get; set; } //id z igdb
    public string Name { get; set; }
    
    [Column(TypeName = "text")]
    public string Description { get; set; }
                                            
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset? EndTime { get; set; }

    public ICollection<Game> Games { get; set; }

    // moze bedzie potrzebne pozniej
    // public boolean is_user_added {get; set;}
}