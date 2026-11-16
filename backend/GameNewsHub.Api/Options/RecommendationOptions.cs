using System.ComponentModel.DataAnnotations;
using GameNewsHub.Hosting.Configuration;

namespace GameNewsHub.Api.Options;

public class RecommendationOptions : IConfigSection
{
    public static string SectionName => "Recommendation";

    [Range(0f, 1f)]
    public float Threshold { get; init; }

    [Range(1, 50)]
    public int MinResults { get; init; } = 3;

    [Range(1, 100)]
    public int DefaultLimit { get; init; } = 10;
}