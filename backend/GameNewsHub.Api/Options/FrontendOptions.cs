using System.ComponentModel.DataAnnotations;

namespace GameNewsHub.Api.Options;

public class FrontendOptions : IConfigSection
{
    public static string SectionName => "Frontend";
    [Required] public string Url { get; init; } = "";
}