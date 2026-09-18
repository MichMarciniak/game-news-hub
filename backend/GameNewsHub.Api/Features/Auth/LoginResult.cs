using GameNewsHub.Contracts;

namespace GameNewsHub.Api.Features.Auth;

public record TokenResult
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}

public record LoginResult
{
    public TokenResult Tokens { get; init; }
    public ProfileDto User { get; init; }
}