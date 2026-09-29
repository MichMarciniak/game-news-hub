using System.ComponentModel.DataAnnotations;

namespace GameNewsHub.Api.Options;

public class FollowServiceOptions : IConfigSection
{
    public static string SectionName => "FollowService";
    [Required] public TimeSpan Timespan { get; init; }
}