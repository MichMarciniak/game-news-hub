using System.ComponentModel.DataAnnotations;
using GameNewsHub.Hosting.Configuration;

namespace GameNewsHub.Api.Options;

public class FrontendOptions : IConfigSection
{
    public static string SectionName => "Frontend";
    [Required] public string Url { get; init; } = "";
}