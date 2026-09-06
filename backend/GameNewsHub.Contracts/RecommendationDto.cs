namespace GameNewsHub.Contracts;

public record RecommendationDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsFollowed { get; set; }
    public bool ContainsFollowedGame { get; set; }
    public bool ContainsFollowedGenre { get; set; }
    public double Score { get; set; }
}