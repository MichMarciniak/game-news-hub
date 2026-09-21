using GameNewsHub.Api.Extensions;
using GameNewsHub.Api.Features.Shared;
using GameNewsHub.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameNewsHub.Api.Features.Platforms;

[ApiController]
[Route("[controller]")]
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
    
    

    [HttpPost("groups/{groupId}/follow")]
    [Authorize]
    public async Task<IActionResult> FollowGroup(int groupId)
    {
        var userId = User.GetUserId();
        var result = await _service.FollowPlatformGroup(userId, groupId);

        return result.MatchFirst<IActionResult>(
            success => Ok(),
            err => this.ProblemErr(err));
    }

    [HttpDelete("groups/{groupId}/follow")]
    [Authorize]
    public async Task<IActionResult> UnfollowGroup(int groupId)
    {
        var userId = User.GetUserId();
        var result = await _service.FollowPlatformGroup(userId, groupId);

        return result.MatchFirst<IActionResult>(
            success => Ok(),
            err => this.ProblemErr(err));
    }

}