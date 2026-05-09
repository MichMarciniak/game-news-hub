namespace GameNewsHub.Contracts;

public record GameRequest 
{
    public int Id{ get; set; }
    public string Name { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;

    public List<GenreDto>? Genres { get; set; }
    public List<PlatformDto>? Platforms { get; set; }
    public CoverDto? Cover { get; set; }
}

public record GameIdContainer(int Id);

public record GameListResponse 
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public record GameDetailsResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
}