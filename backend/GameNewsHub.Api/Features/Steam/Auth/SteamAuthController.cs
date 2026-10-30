using GameNewsHub.Api.Extensions;
using GameNewsHub.Api.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GameNewsHub.Api.Features.Steam.Auth;

[ApiController]
[Route("steam/auth")]
[Authorize]
public class SteamAuthController : ControllerBase
{
    private readonly SteamAuthService _authService;
    private readonly FrontendOptions _frontendOptions;

    public SteamAuthController(
        SteamAuthService steamAuthService,
        IOptions<FrontendOptions> frontendOptions
    )
    {
        _authService = steamAuthService;
        _frontendOptions = frontendOptions.Value;
    }

    [HttpGet("connect")]
    public async Task<ActionResult<string>> GetSteamLoginUrl()
    {
        var result = _authService.GetLoginUrl();

        return result;
    }

    
    [HttpGet("connect/redirect")]
    [ProducesResponseType(StatusCodes.Status302Found)]
    public async Task<IActionResult> GetSteamLoginUrlRedirect()
    {
        var result = _authService.GetLoginUrl();

        return Redirect(result);
    }
    
    [HttpGet("callback")]
    [ProducesResponseType(StatusCodes.Status302Found)]
    public async Task<IActionResult> GetSteamCallback()
    {
        var userId = User.GetId();
        
        var result = await _authService.Login(userId, Request.Query);

        return result.Match(
            _ => Redirect($"{_frontendOptions.Url}me?steam=connected"),
            err => Redirect($"{_frontendOptions.Url}me?steam=error={err[0].Code}&{err[0].Description}")
        );
    }
}