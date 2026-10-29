using GameNewsHub.Api.Extensions;
using GameNewsHub.Api.Features.Shared;
using GameNewsHub.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameNewsHub.Api.Features.Users.Profile;

[ApiController]
[Authorize]
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
        return result.ToNoContent(this);
    }

    [HttpGet]
    public async Task<ActionResult<FullProfileDto>> GetProfile()
    {
        var userId = User.GetUserId();
        var result = await _service.GetFullProfile(userId);
        return result.ToActionResult(this);
    }

    [HttpGet("full")]
    public async Task<ActionResult<FullProfileDto>> GetFullProfile()
    {
        var userId = User.GetUserId();
        var result = await _service.GetFullProfile(userId);
        return result.ToActionResult(this);
    }
}