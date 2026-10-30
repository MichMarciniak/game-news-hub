using GameNewsHub.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameNewsHub.Api.Features.Genres.Follow;

[ApiController]
[Route("genres")]
[Authorize]
public class FollowGenreController : ControllerBase
{

    private readonly FollowGenreService _service;
    
    public FollowGenreController(
        FollowGenreService service
    )
    {
        _service = service;
    }
    
    
    [HttpPost("{id}/follow")]
    public async Task<IActionResult> PostFollowGenre(int id)
    {
        var userId = User.GetId();
        var result = await _service.Follow(userId, id);
        return result.ToNoContent(this);
    }

    [HttpDelete("{id}/follow")]
    public async Task<IActionResult> DeleteFollowGenre(int id)
    {
        var userId = User.GetId();
        var result = await _service.Unfollow(userId, id);
        return result.ToNoContent(this);
    }
}