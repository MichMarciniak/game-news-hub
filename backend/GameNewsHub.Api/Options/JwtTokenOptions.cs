using System.ComponentModel.DataAnnotations;

namespace GameNewsHub.Api.Options;

public class JwtTokenOptions : IConfigSection
{
    public static string SectionName => "Token";
    [Required] public string SigningKey { get; set; }
    [Required] public string Audience { get; set; }
    [Required] public string Issuer { get; set; }
    [Required] public TimeSpan AccessTokenLifetime { get; set; }
    [Required] public TimeSpan AdminAccessTokenLifetime { get; set; }
    [Required] public TimeSpan RefreshTokenLifetime { get; set; }
}