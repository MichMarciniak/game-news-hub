using GameNewsHub.Data.Entities;

namespace Data.Entities;

public class Platform
{
    public int Id { get; set; }
    public int IgdbId { get; set; }
    public string Name { get; set; } = string.Empty;

    public int PlatformGroupId { get; set; }
    public PlatformGroup PlatformGroup { get; set; }
    public ICollection<Game> Games { get; set; } = new List<Game>();
}
