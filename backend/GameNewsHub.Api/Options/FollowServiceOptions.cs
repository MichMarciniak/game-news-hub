using System.ComponentModel.DataAnnotations;
using GameNewsHub.Hosting.Configuration;

namespace GameNewsHub.Api.Options;

public class FollowServiceOptions : IConfigSection
{
    public static string SectionName => "FollowService";
    [Required] public TimeSpan Timespan { get; init; }
}