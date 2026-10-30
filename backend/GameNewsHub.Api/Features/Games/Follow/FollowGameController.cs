using GameNewsHub.Api.Extensions;
using GameNewsHub.Api.Features.Shared;
using GameNewsHub.Contracts;
using GameNewsHub.Contracts.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameNewsHub.Api.Features.Games.Follow;

[ApiController]
[Route("games")]
[Authorize]
public class FollowGameController : ControllerBase
{
    private readonly FollowGameService _service;

    public FollowGameController(
        FollowGameService service
    )
    {
        _service = service;
    }
    
    [HttpPost("{id}/follow")]
    public async Task<IActionResult> PostFollowGame(int id)
    {
        var userId = User.GetId();
        var result = await _service.Follow(userId, id);
        return result.ToNoContent(this);
    }

    [HttpDelete("{id}/follow")]
    public async Task<IActionResult> DeleteFollowGame(int id)
    {
        var userId = User.GetId();
        var result = await _service.Unfollow(userId, id);
        return result.ToNoContent(this);
    }

    [HttpPost("batch/follow")]
    public async Task<IActionResult> PostBatchFollowGames([FromBody] BatchFollowDto<int> gameIds)
    {
        var userId = User.GetId();
        var result = await _service.FollowBatch(userId, gameIds.Ids);
        return result.ToNoContent(this);
    }
    
}