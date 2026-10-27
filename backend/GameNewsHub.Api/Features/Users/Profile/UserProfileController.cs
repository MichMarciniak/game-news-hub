using GameNewsHub.Api.Extensions;
using GameNewsHub.Api.Features.Shared;
using GameNewsHub.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GameNewsHub.Api.Features.Users.Profile;

public class UserProfileController : UserControllerBase 
{
    private readonly UserProfileService _service;

    public UserProfileController(UserProfileService service)
    {
        _service = service;
    }

    [HttpPatch("settings")]
    public async Task<IActionResult> PatchSettings(SettingsDto settings)
    {
        var userId = User.GetUserId();
        var result = await _service.PatchSettings(userId, settings);
        return result.Match(
            details => Ok(details),
            err => this.ProblemErr(err));
            
    }


    [HttpGet]
    public async Task<ActionResult<ProfileDto>> GetProfile()
    {
        var userId = User.GetUserId();
        var result = await _service.GetFullProfile(userId);
        return result.MatchFirst(
            details => Ok(details),
            err => this.ProblemErr(err));
    }
    
    [HttpGet("full")]
    public async Task<ActionResult<ProfileDto>> GetFullProfile()
    {
        var userId = User.GetUserId();
        var result = await _service.GetFullProfile(userId);
        return result.MatchFirst(
            details => Ok(details),
            err => this.ProblemErr(err));
    }

}