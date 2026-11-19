using System.Security.Claims;
using ErrorOr;
using GameNewsHub.Api.Extensions;
using GameNewsHub.Api.Options;
using GameNewsHub.Contracts.Auth;
using GameNewsHub.Contracts.User;
using GameNewsHub.Data.Constants;
using GameNewsHub.Data.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace GameNewsHub.Api.Features.Google.Auth;

[ApiController]
[Route("auth/google")]
public class GoogleAuthController : ControllerBase
{
    private readonly FrontendOptions _frontendOptions;
    private readonly GoogleAuthOptions _googleOptions;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public GoogleAuthController(
        IOptions<FrontendOptions> frontendOptions,
        IOptions<GoogleAuthOptions> googleOptions,
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager)
    {
        _frontendOptions = frontendOptions.Value;
        _googleOptions = googleOptions.Value;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet("login/redirect")]
    [EnableRateLimiting("AuthStrict")]
    public IActionResult GetLoginUrlRedirect()
    {
        var props = new AuthenticationProperties
        {
            RedirectUri = "/auth/google/callback",
            Items =
            {
                { "LoginProvider", "Google" }
            }
        };
        return Challenge(props, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("callback")]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [EnableRateLimiting("AuthStrict")]
    public async Task<IActionResult> Callback()
    {
        // Get the external authentication result using Identity's external scheme
        var result = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
        if (!result.Succeeded || result.Principal == null)
        {
            return Redirect($"{_frontendOptions.Url}me?google=error&code=Google.AuthFailed&message=External authentication failed");
        }

        // Extract user info from Google's claims
        var googleId = result.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = result.Principal.FindFirstValue(ClaimTypes.Email)?.ToLowerInvariant();
        var name = result.Principal.FindFirstValue(ClaimTypes.Name);

        if (string.IsNullOrEmpty(email))
        {
            return Redirect($"{_frontendOptions.Url}me?google=error&code=Google.EmailMissing&message=Email not provided by Google");
        }

        // Check if Google account is already linked to a local user
        var existingLogin = await _userManager.FindByLoginAsync("Google", googleId);
        if (existingLogin != null)
        {
            await _signInManager.ExternalLoginSignInAsync("Google", googleId, isPersistent: true);
            return Redirect($"{_frontendOptions.Url}me?google=connected");
        }

        // Check if email is already taken by a user with a different Google account
        var existingUser = await _userManager.FindByEmailAsync(email);

        if (existingUser != null)
        {
            var existingLogins = await _userManager.GetLoginsAsync(existingUser);
            var hasGoogleLogin = existingLogins.Any(l => l.LoginProvider == "Google" && l.ProviderKey == googleId);

            if (hasGoogleLogin)
            {
                await _signInManager.ExternalLoginSignInAsync("Google", googleId, isPersistent: true);
                return Redirect($"{_frontendOptions.Url}me?google=connected");
            }

            // Link Google account to existing local account
            await _userManager.AddLoginAsync(existingUser, new UserLoginInfo("Google", googleId, "Google"));
            if (!string.IsNullOrEmpty(name) && string.IsNullOrEmpty(existingUser.Name))
            {
                existingUser.Name = name;
                await _userManager.UpdateAsync(existingUser);
            }
            await _signInManager.SignInAsync(existingUser, isPersistent: true);
            return Redirect($"{_frontendOptions.Url}me?google=connected");
        }

        // Create new user
        var newUser = new AppUser
        {
            Email = email,
            UserName = email,
            Name = name,
            EmailConfirmed = true,
            CreatedAt = DateTimeOffset.UtcNow
        };

        var createResult = await _userManager.CreateAsync(newUser);
        if (!createResult.Succeeded)
        {
            return Redirect($"{_frontendOptions.Url}me?google=error&code=Google.SignupFailed&message=Failed to create user");
        }

        await _userManager.AddLoginAsync(newUser, new UserLoginInfo("Google", googleId, "Google"));
        await _userManager.AddToRoleAsync(newUser, Roles.User);
        await _signInManager.SignInAsync(newUser, isPersistent: true);

        return Redirect($"{_frontendOptions.Url}me?google=connected");
    }

    [HttpPost("link")]
    [Authorize]
    [EnableRateLimiting("AuthModerate")]
    public async Task<IActionResult> LinkAccount([FromBody] GoogleLinkRequest request)
    {
        var userId = User.GetId();
        var result = await LinkGoogleAccountAsync(userId, request.Code);
        
        if (result.IsError)
        {
            var error = result.Errors[0];
            return error.Type == ErrorType.Conflict ? Conflict(error) : BadRequest(error);
        }
        
        return NoContent();
    }

    [HttpGet("link/redirect")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [EnableRateLimiting("AuthModerate")]
    public IActionResult LinkAccountRedirect()
    {
        var state = $"link:{User.GetId()}";
        var props = new AuthenticationProperties
        {
            RedirectUri = $"{_frontendOptions.Url}me?google=linked",
            Items =
            {
                { "LoginProvider", "Google" },
                { "state", state }
            }
        };
        return Challenge(props, GoogleDefaults.AuthenticationScheme);
    }

    private async Task<ErrorOr<Success>> LinkGoogleAccountAsync(int userId, string code)
    {
        var tokenRequest = new Dictionary<string, string>
        {
            ["code"] = code,
            ["client_id"] = _googleOptions.ClientId,
            ["client_secret"] = _googleOptions.ClientSecret,
            ["redirect_uri"] = $"{_frontendOptions.ApiUrl}{_googleOptions.CallbackPath}",
            ["grant_type"] = "authorization_code"
        };

        using var httpClient = new HttpClient();
        var response = await httpClient.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(tokenRequest));
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return Error.Failure("Google.TokenExchangeFailed", "Failed to exchange code for tokens");
        }

        var tokenResponse = System.Text.Json.JsonSerializer.Deserialize<GoogleTokenResponse>(content, new System.Text.Json.JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
        {
            return Error.Failure("Google.InvalidTokenResponse", "Invalid token response from Google");
        }

        using var userInfoRequest = new HttpRequestMessage(HttpMethod.Get, "https://www.googleapis.com/oauth2/v2/userinfo");
        userInfoRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
        var userInfoResponse = await httpClient.SendAsync(userInfoRequest);
        var userInfoContent = await userInfoResponse.Content.ReadAsStringAsync();

        if (!userInfoResponse.IsSuccessStatusCode)
        {
            return Error.Failure("Google.UserInfoFailed", "Failed to get user info from Google");
        }

        var googleUser = System.Text.Json.JsonSerializer.Deserialize<GoogleUserInfo>(userInfoContent, new System.Text.Json.JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (googleUser == null)
        {
            return Error.Failure("Google.InvalidUserInfo", "Invalid user info response from Google");
        }

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

        var existingLogins = await _userManager.GetLoginsAsync(user);
        var hasGoogleLogin = existingLogins.Any(l => l.LoginProvider == "Google" && l.ProviderKey == googleUser.Id);

        if (!hasGoogleLogin)
        {
            await _userManager.AddLoginAsync(user, new UserLoginInfo("Google", googleUser.Id, "Google"));
        }

        if (!string.IsNullOrEmpty(googleUser.Name) && string.IsNullOrEmpty(user.Name))
        {
            user.Name = googleUser.Name;
            await _userManager.UpdateAsync(user);
        }

        return Result.Success;
    }

}