namespace GameNewsHub.Api.Dtos;


public record EventRecommendationResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsFollowed { get; set; }
    public bool ContainsFollowedGame { get; set; }
    public double MatchScore { get; set; }
    public double FinalPriority { get; set; }
}