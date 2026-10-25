using System.Security.Claims;
using ErrorOr;
using GameNewsHub.Api.Extensions;
using GameNewsHub.Api.Features.Shared;
using GameNewsHub.Api.Options;
using GameNewsHub.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GameNewsHub.Api.Features.Steam;

[ApiController]
[Route("[controller]")]
[Authorize]
public class SteamController : ControllerBase
{
    private readonly SteamService _service;
    private readonly SteamAuthService _authService;
    private readonly FrontendOptions _frontendOptions;

    public SteamController(
        SteamService service,
        SteamAuthService steamAuthService,
        IOptions<FrontendOptions> frontendOptions
    )
    {
        _service = service;
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
    public async Task<IActionResult> GetSteamLoginUrlRedirect()
    {
        var result = _authService.GetLoginUrl();

        return Redirect(result);
    }
    
    [HttpGet("callback")]
    public async Task<IActionResult> SteamCallback()
    {
        var userId = User.GetUserId();
        
        var result = await _authService.Login(userId, Request.Query);

        return result.Match(
            _ => Redirect($"{_frontendOptions.Url}profile?steam=connected"),
            _ => Redirect($"{_frontendOptions.Url}profile?steam=error")
        );
    }
    
    
    
    
}