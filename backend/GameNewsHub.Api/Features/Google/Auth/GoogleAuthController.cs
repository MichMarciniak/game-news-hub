using System.Security.Claims;
using GameNewsHub.Api.Extensions;
using GameNewsHub.Api.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace GameNewsHub.Api.Features.Google.Auth;

[ApiController]
[Route("auth/google")]
public class GoogleAuthController : ControllerBase
{
    private readonly FrontendOptions _frontendOptions;
    private readonly GoogleAuthService _googleAuthService;

    public GoogleAuthController(
        IOptions<FrontendOptions> frontendOptions,
        GoogleAuthService googleAuthService)
    {
        _frontendOptions = frontendOptions.Value;
        _googleAuthService = googleAuthService;
    }

    [HttpGet("login/redirect")]
    [EnableRateLimiting("AuthStrict")]
    public IActionResult GetLoginUrlRedirect()
    {
        var props = _googleAuthService.GetLoginRedirectProperties();
        return Challenge(props, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("callback")]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [EnableRateLimiting("AuthStrict")]
    public async Task<IActionResult> Callback()
    {
        var result = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);

        GoogleCallbackOutcome outcome;
        try
        {
            outcome = await _googleAuthService.ProcessLoginCallbackAsync(result);

            if (outcome.Kind is "connected" or "create")
            {
                outcome = await _googleAuthService.CompleteSignInAsync(outcome);
            }
        }
        catch (Exception)
        {
            outcome = new GoogleCallbackOutcome
            {
                Kind = "error",
                Code = "Google.ServerError",
                Message = "An error occurred while processing Google authentication"
            };
        }
        finally
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        }

        return Redirect(BuildRedirectUrl(outcome));
    }

    [HttpGet("link/redirect")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [EnableRateLimiting("AuthModerate")]
    public IActionResult LinkAccountRedirect()
    {
        var userId = User.GetId();
        var props = _googleAuthService.GetLinkRedirectProperties(userId);
        return Challenge(props, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("link/callback")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [EnableRateLimiting("AuthModerate")]
    public async Task<IActionResult> LinkCallback()
    {
        var result = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
        var userId = User.GetId();

        GoogleCallbackOutcome outcome;
        try
        {
            if (!result.Succeeded || result.Principal == null)
            {
                outcome = new GoogleCallbackOutcome
                {
                    Kind = "error",
                    Code = "Google.AuthFailed",
                    Message = "External authentication failed"
                };
            }
            else
            {
                outcome = await _googleAuthService.ProcessLinkCallbackAsync(userId, result);
            }
        }
        catch (Exception)
        {
            outcome = new GoogleCallbackOutcome
            {
                Kind = "error",
                Code = "Google.ServerError",
                Message = "An error occurred while linking your Google account"
            };
        }
        finally
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        }

        return Redirect(BuildRedirectUrl(outcome));
    }

    private string BuildRedirectUrl(GoogleCallbackOutcome outcome)
    {
        var parameters = new Dictionary<string, string?> { ["google"] = outcome.Kind };

        if (!string.IsNullOrEmpty(outcome.Code))
        {
            parameters["code"] = outcome.Code;
        }

        if (!string.IsNullOrEmpty(outcome.Message))
        {
            parameters["message"] = outcome.Message;
        }

        return QueryHelpers.AddQueryString($"{_frontendOptions.Url}auth/callback", parameters);
    }
}
