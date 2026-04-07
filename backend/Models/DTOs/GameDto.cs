namespace backend.Models.DTOs;

public record GameResponse
{
    public int Id{ get; set; }
    public string Name { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;

    public List<GenreDto>? Genres { get; set; }
    public List<PlatformDto>? Platforms { get; set; }
    public CoverDto? Cover { get; set; }
}