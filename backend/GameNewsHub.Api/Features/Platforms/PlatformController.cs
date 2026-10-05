using GameNewsHub.Api.Extensions;
using GameNewsHub.Api.Features.Shared;
using GameNewsHub.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameNewsHub.Api.Features.Platforms;

[ApiController]
[Route("platforms")]
public class PlatformController : ControllerBase
{
    private readonly PlatformService _service;

    public PlatformController(PlatformService service)
    {
        _service = service;
    }

    [HttpGet("groups")]
    public async Task<ActionResult<List<PlatformGroupDto>>> GetGroups()
    {
        var result = await _service.GetPlatformGroups();

        return result;
    }

    [HttpGet("groups/full")]
    public async Task<ActionResult<List<PlatformGroupWithPlatformsDto>>> GetGroupsFull()
    {
        var result = await _service.GetPlatformGroupsFull();

        return result;
    }
    
    

    [HttpPost("groups/{id}/follow")]
    [Authorize]
    public async Task<IActionResult> FollowGroup(int id)
    {
        var userId = User.GetUserId();
        var result = await _service.FollowPlatformGroup(userId, id);

        return result.MatchFirst<IActionResult>(
            success => Ok(),
            err => this.ProblemErr(err));
    }

    [HttpDelete("groups/{id}/follow")]
    [Authorize]
    public async Task<IActionResult> UnfollowGroup(int id)
    {
        var userId = User.GetUserId();
        var result = await _service.UnfollowPlatformGroup(userId, id);

        return result.MatchFirst<IActionResult>(
            success => Ok(),
            err => this.ProblemErr(err));
    }

}