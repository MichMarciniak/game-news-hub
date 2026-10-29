using GameNewsHub.Api.Extensions;
using GameNewsHub.Api.Features.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameNewsHub.Api.Features.Platforms.Follow;

[ApiController]
[Route("platforms")]
[Authorize]
public class FollowPlatformGroupController : ControllerBase
{
    private readonly FollowPlatformGroupService _service;

    public FollowPlatformGroupController(
        FollowPlatformGroupService service
    )
    {
        _service = service;
    }
    
    [HttpPost("groups/{id}/follow")]
    public async Task<IActionResult> PostFollowGroup(int id)
    {
        var userId = User.GetUserId();
        var result = await _service.FollowPlatformGroup(userId, id);
        return result.ToNoContent(this);
    }

    [HttpDelete("groups/{id}/follow")]
    public async Task<IActionResult> DeleteFollowGroup(int id)
    {
        var userId = User.GetUserId();
        var result = await _service.UnfollowPlatformGroup(userId, id);
        return result.ToNoContent(this);
    }
}