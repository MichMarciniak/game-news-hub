namespace GameNewsHub.Sync.Dtos;

public record IgdbGameResponse 
{
    public int Id{ get; set; }
    public string Name { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;

    public List<IgdbGenreResponse>? Genres { get; set; }
    public List<IgdbPlatformResponse>? Platforms { get; set; }
    public IgdbCoverResponse? Cover { get; set; }
}

public record GameIdContainer(int Id);
