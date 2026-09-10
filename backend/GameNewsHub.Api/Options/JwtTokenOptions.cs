namespace backend.Configuration;

public class JwtTokenOptions
{
    public const string SectionName = "Token";
    public string SigningKey { get; set; }
    public string Audience { get; set; }
    public string Issuer { get; set; }
    public TimeSpan AccessTokenLifetime { get; set; }
    public TimeSpan AdminAccessTokenLifetime { get; set; }
    public TimeSpan RefreshTokenLifetime { get; set; }

}