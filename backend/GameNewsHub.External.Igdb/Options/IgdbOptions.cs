using System.ComponentModel.DataAnnotations;
using GameNewsHub.Hosting.Configuration;

namespace GameNewsHub.External.Igdb.Options;

public class IgdbOptions : IConfigSection
{
    public static string SectionName => "Igdb";
    [Required] public string BaseUrl { get; set; }
    [Required] public string TokenUrl { get; set; }
    [Required] public string ClientId { get; set; }
    [Required] public string ClientSecret { get; set; }
    [Required] public string Mode { get; set; } = "Standard";
}