using GameNewsHub.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameNewsHub.Api.Features.Events.Follow;

[ApiController]
[Route("events")]
[Authorize]
public class FollowEventController : ControllerBase
{
    private readonly FollowEventService _service;

    public FollowEventController(
        FollowEventService service
    )
    {
        _service = service;
    }
    
    [HttpPost("{id}/follow")]
    public async Task<IActionResult> PostFollowEvent(int id)
    {
        var userId = User.GetId();
        var result = await _service.Follow(userId, id);
        return result.ToNoContent(this);
    }

    [HttpDelete("{id}/follow")]
    public async Task<IActionResult> DeleteFollowEvent(int id)
    {
        var userId = User.GetId();
        var result = await _service.Unfollow(userId, id);
        return result.ToNoContent(this);
    }
    

}