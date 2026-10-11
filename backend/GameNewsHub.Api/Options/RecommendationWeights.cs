using System.ComponentModel.DataAnnotations;
using GameNewsHub.Hosting.Configuration;

namespace GameNewsHub.Api.Options;

public class RecommendationWeights : IConfigSection
{
    public static string SectionName => "RecommendationWeights";
    [Required] public double FollowedEventBonus { get; set; }
    [Required] public double FollowedGameBonus { get; set; }
    [Required] public double FollowedPlatformBonus { get; set; }
    [Required] public double GenreMatchMultiplier { get; set; }
}