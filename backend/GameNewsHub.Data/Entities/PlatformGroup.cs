using GameNewsHub.Data.Entities;

namespace Data.Entities;

public class PlatformGroup
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; 

    public ICollection<Platform> Platforms { get; set; } = new List<Platform>();
}