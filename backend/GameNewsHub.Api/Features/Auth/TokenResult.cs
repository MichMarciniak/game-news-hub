namespace GameNewsHub.Api.Features.Auth;

public record TokenResult
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}