namespace backend.Configuration;

public class RecommendationWeights
{
    public double FollowedEventBonus { get; set; }
    public double FollowedGameBonus { get; set; }
    public double FollowedPlatformBonus { get; set; }
    public double GenreMatchMultiplier { get; set; }
}