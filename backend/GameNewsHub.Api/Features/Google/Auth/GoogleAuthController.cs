using ErrorOr;
using GameNewsHub.Api.Extensions;
using GameNewsHub.Api.Options;
using GameNewsHub.Contracts.Auth;
using GameNewsHub.Contracts.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace GameNewsHub.Api.Features.Google.Auth;

[ApiController]
[Route("auth/google")]
public class GoogleAuthController : ControllerBase
{
    private readonly GoogleAuthService _authService;
    private readonly FrontendOptions _frontendOptions;

    public GoogleAuthController(
        GoogleAuthService authService,
        IOptions<FrontendOptions> frontendOptions)
    {
        _authService = authService;
        _frontendOptions = frontendOptions.Value;
    }

    [HttpGet("login")]
    [EnableRateLimiting("AuthStrict")]
    public ActionResult<string> GetLoginUrl()
    {
        var url = _authService.GetAuthorizationUrl();
        return Ok(new { url });
    }

    [HttpGet("login/redirect")]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [EnableRateLimiting("AuthStrict")]
    public IActionResult GetLoginUrlRedirect()
    {
        var url = _authService.GetAuthorizationUrl();
        return Redirect(url);
    }

    [HttpGet("callback")]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [EnableRateLimiting("AuthStrict")]
    public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string? state)
    {
        // Check if this is a link request (state starts with "link:")
        bool isLink = !string.IsNullOrEmpty(state) && state.StartsWith("link:");
        
        // For link flow, state is required for CSRF protection
        if (isLink && string.IsNullOrEmpty(state))
        {
            return Redirect($"{_frontendOptions.Url}me?google=error&code=Google.InvalidState&message=Missing state parameter");
        }

        ErrorOr<ProfileDto> result;
        if (isLink)
        {
            // For link flow, user must be authenticated
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return Redirect($"{_frontendOptions.Url}me?google=error&code=Google.Unauthorized&message=Must be logged in to link account");
            }
            
            var userId = User.GetId();
            var linkResult = await _authService.LinkGoogleAccountAsync(userId, code);
            if (linkResult.IsError)
            {
                var error = linkResult.Errors[0];
                return Redirect($"{_frontendOptions.Url}me?google=error&code={error.Code}&message={Uri.EscapeDataString(error.Description)}");
            }
            
            return Redirect($"{_frontendOptions.Url}me?google=linked");
        }
        else
        {
            // Regular login/signup flow - state is optional
            result = await _authService.HandleCallbackAsync(code, state);
        }

        return result.Match(
            _ => Redirect($"{_frontendOptions.Url}me?google=connected"),
            err => Redirect($"{_frontendOptions.Url}me?google=error&code={err[0].Code}&message={Uri.EscapeDataString(err[0].Description)}")
        );
    }

    [HttpPost("link")]
    [Authorize]
    [EnableRateLimiting("AuthModerate")]
    public async Task<IActionResult> LinkAccount([FromBody] GoogleLinkRequest request)
    {
        var userId = User.GetId();
        var result = await _authService.LinkGoogleAccountAsync(userId, request.Code);
        
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
        var url = _authService.GetAuthorizationUrl(state);
        return Redirect(url);
    }
}