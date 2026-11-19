using System.Text.Json.Serialization;

namespace GameNewsHub.Api.Features.Google.Auth;

public record GoogleTokenResponse(
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("token_type")] string TokenType,
    [property: JsonPropertyName("expires_in")] int ExpiresIn,
    [property: JsonPropertyName("refresh_token")] string? RefreshToken,
    [property: JsonPropertyName("scope")] string? Scope,
    [property: JsonPropertyName("id_token")] string? IdToken
);

public record GoogleUserInfo(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("verified_email")] bool? VerifiedEmail,
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("given_name")] string? GivenName,
    [property: JsonPropertyName("family_name")] string? FamilyName,
    [property: JsonPropertyName("picture")] string? Picture,
    [property: JsonPropertyName("locale")] string? Locale
);
