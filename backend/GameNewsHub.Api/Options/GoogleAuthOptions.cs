using System.ComponentModel.DataAnnotations;
using GameNewsHub.Hosting.Configuration;

namespace GameNewsHub.Api.Options;

public class GoogleAuthOptions : IConfigSection
{
    public static string SectionName => "GoogleAuth";
    [Required] public string ClientId { get; init; } = "";
    [Required] public string ClientSecret { get; init; } = "";
    [Required] public string RedirectUri { get; init; } = "";
}