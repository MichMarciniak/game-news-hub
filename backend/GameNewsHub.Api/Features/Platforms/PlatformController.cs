using System.Net;
using backend.Extensions;
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

    [HttpPost("groups/{groupId}/follow")]
    [Authorize]
    public async Task<IActionResult> FollowGroup(int groupId)
    {
        var userId = User.GetUserId();
        var result = await _service.FollowPlatformGroup(userId, groupId);

        return result.Match<IActionResult>(
            success => Ok(),
            errors => Problem(errors[0].Description));
    }

    [HttpDelete("groups/{groupId}/follow")]
    [Authorize]
    public async Task<IActionResult> UnfollowGroup(int groupId)
    {
        var userId = User.GetUserId();
        var result = await _service.FollowPlatformGroup(userId, groupId);

        return result.Match<IActionResult>(
            success => Ok(),
            errors => Problem(errors[0].Description));
    }

    [HttpPatch("reassign")]
    [Authorize] // role admin?
    public async Task<IActionResult> ReassignPlatformGroup([FromQuery] int platformId, [FromQuery] int groupId)
    {
        var result = await _service.ReassignPlatformGroup(platformId, groupId);
        
        return result.Match<IActionResult>(
            success => Ok(),
            errors => Problem(errors[0].Description));
    }


}