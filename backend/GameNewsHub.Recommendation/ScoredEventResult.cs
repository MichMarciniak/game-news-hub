namespace GameNewsHub.Recommendation;

public class ScoredEventResult
{
    public EventVector EventVector { get; set; }
    public float FinalScore { get; set; }

    public float PersonalScore { get; set; }
    public float PopularityScore { get; set; }
    public float MomentumScore { get; set; }

    public float GameFacetContribution { get; set; }
    public float GenreFacetContribution { get; set; }
    public float PlatformGroupFacetContribution { get; set; }
    public float SeriesFacetContribution { get; set; }

    public List<RecLabel> Labels { get; set; } = [];
}