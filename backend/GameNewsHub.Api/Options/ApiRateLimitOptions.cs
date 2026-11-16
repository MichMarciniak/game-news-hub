using System.ComponentModel.DataAnnotations;
using GameNewsHub.Hosting.Configuration;

namespace GameNewsHub.Api.Options;

public class ApiRateLimitOptions : IConfigSection
{
    public static string SectionName => "ApiRateLimits";

    public SteamApiLimits Steam { get; init; } = new();
}

public class SteamApiLimits
{
    [Required] public int PermitLimit { get; init; } = 10;
    [Required] public int WindowDays { get; init; } = 1;
}