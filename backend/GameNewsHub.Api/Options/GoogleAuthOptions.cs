using System.ComponentModel.DataAnnotations;
using GameNewsHub.Hosting.Configuration;

namespace GameNewsHub.Api.Options;

public class GoogleAuthOptions : IConfigSection
{
    public static string SectionName => "GoogleAuth";
    [Required] public string ClientId { get; init; } = "";
    [Required] public string ClientSecret { get; init; } = "";
    public string CallbackPath { get; init; } = "/auth/google/callback";
}