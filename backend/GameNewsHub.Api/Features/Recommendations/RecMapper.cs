using GameNewsHub.Contracts.Recommendations;
using GameNewsHub.Data.Entities;
using GameNewsHub.Recommendation;
using Riok.Mapperly.Abstractions;

namespace GameNewsHub.Api.Features.Recommendations;

[Mapper]
public partial class RecMapper
{
    public RecommendationDto MapToRecommendation(ScoredEventResult scored, Event evt) => new()
    {
        Id = evt.Id,
        Name = evt.Name,
        FollowerCount = evt.FollowerCount,
        StartTime = evt.StartTime,
        EndTime = evt.EndTime,
        Score = MapToScoreDto(scored)
    };


    public RecommendationScoreDto MapToScoreDto(ScoredEventResult scored) => new()
    {
        FinalScore = scored.FinalScore,
        PopularityScore = scored.PopularityScore,
        Labels = scored.Labels.Select(MapToRecoLabel).ToList(),
        Details = MapToDetailsDto(scored)
    };

    public partial DetailedScoreDto MapToDetailsDto(ScoredEventResult scored);

    public partial RecoLabel MapToRecoLabel(RecLabel label);

}