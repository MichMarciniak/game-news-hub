using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using ErrorOr;
using GameNewsHub.Api.Constants;
using GameNewsHub.Api.Extensions;
using GameNewsHub.Api.Features.Users.Profile;
using GameNewsHub.Api.Options;
using GameNewsHub.Contracts.Auth;
using GameNewsHub.Contracts.User;
using GameNewsHub.Data.Constants;
using GameNewsHub.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace GameNewsHub.Api.Features.Google.Auth;

public class GoogleAuthService
{
    private readonly HttpClient _httpClient;
    private readonly GoogleAuthOptions _options;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ILogger<GoogleAuthService> _logger;

    public GoogleAuthService(
        HttpClient httpClient,
        IOptions<GoogleAuthOptions> options,
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        ILogger<GoogleAuthService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    private string BuildRedirectUri()
    {
        return _options.RedirectUri;
    }

    public string GetAuthorizationUrl(string? state = null)
    {
        var scopes = "openid email profile";
        var redirectUri = BuildRedirectUri();
        var url = $"https://accounts.google.com/o/oauth2/v2/auth?" +
                  $"client_id={_options.ClientId}" +
                  $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                  $"&response_type=code" +
                  $"&scope={Uri.EscapeDataString(scopes)}" +
                  $"&access_type=offline" +
                  $"&prompt=consent";
        
        if (!string.IsNullOrEmpty(state))
        {
            url += $"&state={Uri.EscapeDataString(state)}";
        }

        return url;
    }

    public async Task<ErrorOr<GoogleTokenResponse>> ExchangeCodeForTokensAsync(string code)
    {
        var tokenRequest = new Dictionary<string, string>
        {
            ["code"] = code,
            ["client_id"] = _options.ClientId,
            ["client_secret"] = _options.ClientSecret,
            ["redirect_uri"] = BuildRedirectUri(),
            ["grant_type"] = "authorization_code"
        };

        var response = await _httpClient.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(tokenRequest));
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Google token exchange failed: {Content}", content);
            return Error.Failure("Google.TokenExchangeFailed", "Failed to exchange code for tokens");
        }

        var tokenResponse = JsonSerializer.Deserialize<GoogleTokenResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (tokenResponse == null)
        {
            return Error.Failure("Google.InvalidTokenResponse", "Invalid token response from Google");
        }

        if (string.IsNullOrEmpty(tokenResponse.AccessToken))
        {
            return Error.Failure("Google.MissingAccessToken", "No access token in response from Google");
        }

        return tokenResponse;
    }

    public async Task<ErrorOr<GoogleUserInfo>> GetUserInfoAsync(string accessToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://www.googleapis.com/oauth2/v2/userinfo");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await _httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Google user info request failed: {Content}", content);
            return Error.Failure("Google.UserInfoFailed", "Failed to get user info from Google");
        }

        var userInfo = JsonSerializer.Deserialize<GoogleUserInfo>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (userInfo == null)
        {
            return Error.Failure("Google.InvalidUserInfo", "Invalid user info response from Google");
        }

        return userInfo;
    }

    public async Task<ErrorOr<ProfileDto>> HandleCallbackAsync(string code, string? state = null)
    {
        var tokenResult = await ExchangeCodeForTokensAsync(code);
        if (tokenResult.IsError)
        {
            return tokenResult.Errors;
        }

        var userInfoResult = await GetUserInfoAsync(tokenResult.Value.AccessToken);
        if (userInfoResult.IsError)
        {
            return userInfoResult.Errors;
        }

        var googleUser = userInfoResult.Value;
        var email = googleUser.Email?.ToLowerInvariant();

        if (string.IsNullOrEmpty(email))
        {
            return Error.Validation("Google.EmailMissing", "Email not provided by Google");
        }

        if (!googleUser.VerifiedEmail.GetValueOrDefault(false))
        {
            return Error.Validation("Google.EmailNotVerified", "Email not verified by Google");
        }

        var existingUser = await _userManager.FindByEmailAsync(email);
        
        if (existingUser != null)
        {
            if (string.IsNullOrEmpty(existingUser.GoogleId))
            {
                existingUser.GoogleId = googleUser.Id;
                await _userManager.UpdateAsync(existingUser);
            }
            await _signInManager.SignInAsync(existingUser, isPersistent: true);
            var isAdmin = await _userManager.IsInRoleAsync(existingUser, Roles.Admin);
            _logger.LogInformation("User {UserId} signed in via Google", existingUser.Id);
            
            var settings = ProfileMapper.ToSettingsDto(existingUser);
            var result = ProfileMapper.ToDto(existingUser);
            result.Settings = settings;
            result.IsAdmin = isAdmin;
            return result;
        }

        var newUser = new AppUser
        {
            Email = email,
            UserName = email,
            EmailConfirmed = true,
            CreatedAt = DateTimeOffset.UtcNow
        };

        var createResult = await _userManager.CreateAsync(newUser);
        if (!createResult.Succeeded)
        {
            _logger.LogWarning("Google signup failed for {Email}: {Errors}", email, string.Join(",", createResult.Errors.Select(e => e.Code)));
            return createResult.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();
        }

        await _userManager.AddToRoleAsync(newUser, Roles.User);
        await _signInManager.SignInAsync(newUser, isPersistent: true);

        _logger.LogInformation("New user {UserId} created via Google with email {Email}", newUser.Id, email);

        var newSettings = ProfileMapper.ToSettingsDto(newUser);
        var newResult = ProfileMapper.ToDto(newUser);
        newResult.Settings = newSettings;
        newResult.IsAdmin = false;
        return newResult;
    }

    public async Task<ErrorOr<Success>> LinkGoogleAccountAsync(int userId, string code)
    {
        var tokenResult = await ExchangeCodeForTokensAsync(code);
        if (tokenResult.IsError)
        {
            return tokenResult.Errors;
        }

        var userInfoResult = await GetUserInfoAsync(tokenResult.Value.AccessToken);
        if (userInfoResult.IsError)
        {
            return userInfoResult.Errors;
        }

        var googleUser = userInfoResult.Value;
        var email = googleUser.Email?.ToLowerInvariant();

        if (string.IsNullOrEmpty(email) || !googleUser.VerifiedEmail.GetValueOrDefault(false))
        {
            return Error.Validation("Google.InvalidAccount", "Invalid Google account");
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return Error.NotFound("User.NotFound", "User does not exist");
        }

        if (user.Email?.ToLowerInvariant() != email)
        {
            return Error.Conflict("Google.EmailMismatch", "Google account email does not match your account email");
        }

        user.GoogleId = googleUser.Id;
        await _userManager.UpdateAsync(user);

        _logger.LogInformation("Google account linked for User {UserId}", userId);
        return Result.Success;
    }
}

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