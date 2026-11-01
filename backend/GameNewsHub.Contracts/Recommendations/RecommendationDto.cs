namespace GameNewsHub.Contracts.Recommendations;

public record RecommendationResponse
{
    public float Personalization { get; set; } // alpha = 0 - czysta popularnosc, 0.9 - bardzo personalizowane
    public IReadOnlyList<RecommendationDto> Events { get; set; } = [];
}
public record RecommendationDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int FollowerCount { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset? EndTime { get; set; }

    public RecommendationScoreDto Score { get; set; }
    public bool IsUpcoming { get; set; }
}
public record RecommendationScoreDto
{
    public float FinalScore { get; set; }
    public float PopularityScore { get; set; }
    public List<RecoLabel> Labels { get; set; } = [];
    public DetailedScoreDto Details { get; set; }
}

public record RecoLabel
{
    public string Code { get; set; }
    public int Count { get; set; }
}

public record DetailedScoreDto
{
    public float PersonalScore { get; set; }
    public float MomentumScore { get; set; }
    public float GameFacetContribution { get; set; }
    public float GenreFacetContribution { get; set; }
    public float PlatformGroupFacetContribution { get; set; }
    public float SeriesFacetContribution { get; set; }
}