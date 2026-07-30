namespace GameNewsHub.Api.Features.Games;

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
    public string Cover { get; set; } = string.Empty;
    
}
