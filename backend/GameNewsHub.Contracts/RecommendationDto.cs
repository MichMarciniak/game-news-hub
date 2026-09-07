namespace GameNewsHub.Contracts;

public record RecommendationDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsFollowed { get; set; }
    public bool ContainsFollowedGame { get; set; }
    public bool ContainsFollowedPlatformGroup { get; set; }
    public double MatchScore { get; set; }
    public double FinalPriority { get; set; }
}
