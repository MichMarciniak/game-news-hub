using System.ComponentModel.DataAnnotations;

namespace GameNewsHub.Sync.Options;

public class ApiOptions : IConfigSection
{
    public static string SectionName => "Api";
    [Required] public string BaseUrl { get; set; }
    [Required] public string TokenUrl { get; set; }
    [Required] public string ClientId { get; set; }
    [Required] public string ClientSecret { get; set; }
}