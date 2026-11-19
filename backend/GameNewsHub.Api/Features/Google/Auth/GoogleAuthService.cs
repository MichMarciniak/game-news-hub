using System.Security.Claims;
using System.Security.Cryptography;
using GameNewsHub.Api.Extensions;
using GameNewsHub.Api.Options;
using GameNewsHub.Data;
using GameNewsHub.Data.Constants;
using GameNewsHub.Data.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GameNewsHub.Api.Features.Google.Auth;

public sealed record GoogleCallbackOutcome
{
    public string Kind { get; init; } = "connected";
    public string? Code { get; init; }
    public string? Message { get; init; }
    public string? Email { get; init; }
    public string? Name { get; init; }
    public string? GoogleId { get; init; }
    public bool IsEmailVerified { get; init; }
}

public class GoogleAuthService
{
    private readonly FrontendOptions _frontendOptions;
    private readonly GoogleAuthOptions _googleOptions;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly AppDbContext _dbContext;

    public GoogleAuthService(
        IOptions<FrontendOptions> frontendOptions,
        IOptions<GoogleAuthOptions> googleOptions,
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        AppDbContext dbContext)
    {
        _frontendOptions = frontendOptions.Value;
        _googleOptions = googleOptions.Value;
        _userManager = userManager;
        _signInManager = signInManager;
        _dbContext = dbContext;
    }

    public AuthenticationProperties GetLoginRedirectProperties()
    {
        var state = GenerateStateToken();
        return new AuthenticationProperties
        {
            RedirectUri = "/auth/google/callback",
            Items =
            {
                { "LoginProvider", "Google" },
                { "state", state },
                { "flow", "login" }
            }
        };
    }

    public AuthenticationProperties GetLinkRedirectProperties(int userId)
    {
        var state = GenerateStateToken();
        return new AuthenticationProperties
        {
            RedirectUri = "/auth/google/link/callback",
            Items =
            {
                { "LoginProvider", "Google" },
                { "state", state },
                { "flow", "link" },
                { "linkUserId", userId.ToString() }
            }
        };
    }

    public async Task<GoogleCallbackOutcome> ProcessLoginCallbackAsync(AuthenticateResult externalResult)
    {
        if (!externalResult.Succeeded || externalResult.Principal == null)
        {
            return new GoogleCallbackOutcome
            {
                Kind = "error",
                Code = "Google.AuthFailed",
                Message = "External authentication failed"
            };
        }

        var state = externalResult.Properties?.Items?.TryGetValue("state", out var stateValue) == true
            ? stateValue
            : null;

        if (string.IsNullOrEmpty(state))
        {
            return new GoogleCallbackOutcome
            {
                Kind = "error",
                Code = "Google.InvalidState",
                Message = "Invalid login state"
            };
        }

        var flow = externalResult.Properties?.Items?.TryGetValue("flow", out var flowValue) == true
            ? flowValue
            : null;

        if (flow != "login")
        {
            return new GoogleCallbackOutcome
            {
                Kind = "error",
                Code = "Google.InvalidFlow",
                Message = "Invalid login flow"
            };
        }

        var googleId = externalResult.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = externalResult.Principal.FindFirstValue(ClaimTypes.Email)?.ToLowerInvariant();
        var name = externalResult.Principal.FindFirstValue(ClaimTypes.Name);
        var isEmailVerified = true;

        if (string.IsNullOrEmpty(email))
        {
            return new GoogleCallbackOutcome
            {
                Kind = "error",
                Code = "Google.EmailMissing",
                Message = "Email not provided by Google"
            };
        }

        if (!isEmailVerified)
        {
            return new GoogleCallbackOutcome
            {
                Kind = "error",
                Code = "Google.EmailNotVerified",
                Message = "Google email is not verified"
            };
        }

        if (string.IsNullOrEmpty(googleId))
        {
            return new GoogleCallbackOutcome
            {
                Kind = "error",
                Code = "Google.InvalidId",
                Message = "Google ID not provided"
            };
        }

        var existingLogin = await _userManager.FindByLoginAsync("Google", googleId);
        if (existingLogin != null)
        {
            return new GoogleCallbackOutcome
            {
                Kind = "connected",
                GoogleId = googleId
            };
        }

        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            return new GoogleCallbackOutcome
            {
                Kind = "account_exists",
                Code = "Google.AccountExists",
                Message = "An account with this email already exists. Log in first to link your Google account."
            };
        }

        return new GoogleCallbackOutcome
        {
            Kind = "create",
            Email = email,
            Name = name,
            IsEmailVerified = true,
            GoogleId = googleId
        };
    }

    public async Task<GoogleCallbackOutcome> CompleteSignInAsync(GoogleCallbackOutcome outcome)
    {
        if (outcome.Kind == "connected")
        {
            if (string.IsNullOrEmpty(outcome.GoogleId))
            {
                return new GoogleCallbackOutcome
                {
                    Kind = "error",
                    Code = "Google.InvalidId",
                    Message = "Google ID is required to sign in"
                };
            }

            var signInResult = await _signInManager.ExternalLoginSignInAsync("Google", outcome.GoogleId, isPersistent: true);
            if (!signInResult.Succeeded)
            {
                return new GoogleCallbackOutcome
                {
                    Kind = "error",
                    Code = "Google.SignInFailed",
                    Message = "Failed to sign in with Google account"
                };
            }

            return new GoogleCallbackOutcome { Kind = "connected" };
        }

        if (outcome.Kind == "create")
        {
            return await CreateNewUserAsync(outcome);
        }

        return outcome;
    }

    private async Task<GoogleCallbackOutcome> CreateNewUserAsync(GoogleCallbackOutcome outcome)
    {
        if (string.IsNullOrEmpty(outcome.Email) || string.IsNullOrEmpty(outcome.GoogleId))
        {
            return new GoogleCallbackOutcome
            {
                Kind = "error",
                Code = "Google.InvalidData",
                Message = "Email and Google ID are required to create an account"
            };
        }

        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var newUser = new AppUser
            {
                Email = outcome.Email,
                UserName = outcome.Email,
                Name = outcome.Name,
                EmailConfirmed = outcome.IsEmailVerified,
                CreatedAt = DateTimeOffset.UtcNow
            };

            var createResult = await _userManager.CreateAsync(newUser);
            if (!createResult.Succeeded)
            {
                await transaction.RollbackAsync();
                return new GoogleCallbackOutcome
                {
                    Kind = "error",
                    Code = "Google.SignupFailed",
                    Message = "Failed to create user"
                };
            }

            var addLoginResult = await _userManager.AddLoginAsync(newUser, new UserLoginInfo("Google", outcome.GoogleId, "Google"));
            if (!addLoginResult.Succeeded)
            {
                await transaction.RollbackAsync();
                return new GoogleCallbackOutcome
                {
                    Kind = "error",
                    Code = "Google.LinkFailed",
                    Message = "Failed to link Google account"
                };
            }

            var roleResult = await _userManager.AddToRoleAsync(newUser, Roles.User);
            if (!roleResult.Succeeded)
            {
                await transaction.RollbackAsync();
                return new GoogleCallbackOutcome
                {
                    Kind = "error",
                    Code = "Google.RoleAssignmentFailed",
                    Message = "Failed to assign role"
                };
            }

            await transaction.CommitAsync();

            await _signInManager.SignInAsync(newUser, isPersistent: true);

            return new GoogleCallbackOutcome { Kind = "connected" };
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<GoogleCallbackOutcome> ProcessLinkCallbackAsync(int userId, AuthenticateResult externalResult)
    {
        if (!externalResult.Succeeded || externalResult.Principal == null)
        {
            return new GoogleCallbackOutcome
            {
                Kind = "error",
                Code = "Google.AuthFailed",
                Message = "External authentication failed"
            };
        }

        var state = externalResult.Properties?.Items?.TryGetValue("state", out var stateValue) == true
            ? stateValue
            : null;

        if (string.IsNullOrEmpty(state))
        {
            return new GoogleCallbackOutcome
            {
                Kind = "error",
                Code = "Google.InvalidState",
                Message = "Invalid link state"
            };
        }

        var flow = externalResult.Properties?.Items?.TryGetValue("flow", out var flowValue) == true
            ? flowValue
            : null;

        if (flow != "link")
        {
            return new GoogleCallbackOutcome
            {
                Kind = "error",
                Code = "Google.InvalidFlow",
                Message = "Invalid link flow"
            };
        }

        var linkUserIdStr = externalResult.Properties?.Items?.TryGetValue("linkUserId", out var linkUserIdValue) == true
            ? linkUserIdValue
            : null;

        if (!int.TryParse(linkUserIdStr, out var linkUserId) || linkUserId != userId)
        {
            return new GoogleCallbackOutcome
            {
                Kind = "error",
                Code = "Google.InvalidUserId",
                Message = "Invalid user id"
            };
        }

        var googleId = externalResult.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = externalResult.Principal.FindFirstValue(ClaimTypes.Email)?.ToLowerInvariant();
        var name = externalResult.Principal.FindFirstValue(ClaimTypes.Name);
        var isEmailVerified = IsEmailVerified(externalResult.Principal);

        if (string.IsNullOrEmpty(googleId))
        {
            return new GoogleCallbackOutcome
            {
                Kind = "error",
                Code = "Google.InvalidId",
                Message = "Google ID not provided"
            };
        }

        if (string.IsNullOrEmpty(email))
        {
            return new GoogleCallbackOutcome
            {
                Kind = "error",
                Code = "Google.EmailMissing",
                Message = "Email not provided by Google"
            };
        }

        if (!isEmailVerified)
        {
            return new GoogleCallbackOutcome
            {
                Kind = "error",
                Code = "Google.EmailNotVerified",
                Message = "Google email is not verified"
            };
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return new GoogleCallbackOutcome
            {
                Kind = "error",
                Code = "User.NotFound",
                Message = "User does not exist"
            };
        }

        var existingGoogleLogin = await _userManager.FindByLoginAsync("Google", googleId);
        if (existingGoogleLogin != null)
        {
            if (existingGoogleLogin.Id != user.Id)
            {
                return new GoogleCallbackOutcome
                {
                    Kind = "error",
                    Code = "Google.AccountAlreadyLinked",
                    Message = "Google account is already linked to another local account"
                };
            }

            return new GoogleCallbackOutcome { Kind = "linked" };
        }

        if (user.Email?.ToLowerInvariant() != email)
        {
            return new GoogleCallbackOutcome
            {
                Kind = "error",
                Code = "Google.EmailMismatch",
                Message = "Google account email does not match your account email"
            };
        }

        var addLoginResult = await _userManager.AddLoginAsync(user, new UserLoginInfo("Google", googleId, "Google"));
        if (!addLoginResult.Succeeded)
        {
            return new GoogleCallbackOutcome
            {
                Kind = "error",
                Code = "Google.LinkFailed",
                Message = "Failed to link Google account"
            };
        }

        if (!string.IsNullOrEmpty(name) && string.IsNullOrEmpty(user.Name))
        {
            user.Name = name;
            await _userManager.UpdateAsync(user);
        }

        return new GoogleCallbackOutcome { Kind = "linked" };
    }

    private bool IsEmailVerified(ClaimsPrincipal principal)
    {
        foreach (var claim in _googleOptions.VerifiedEmailClaims)
        {
            var value = principal.FindFirstValue(claim);
            if (value != null && bool.TryParse(value, out var verified) && verified)
            {
                return true;
            }
        }
        return false;
    }

    private static string GenerateStateToken()
    {
        return Convert.ToHexString(RandomNumberGenerator.GetBytes(16)).ToLowerInvariant();
    }
}
