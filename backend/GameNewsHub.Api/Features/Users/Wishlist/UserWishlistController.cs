using GameNewsHub.Api.Extensions;
using GameNewsHub.Api.Features.Shared;
using GameNewsHub.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GameNewsHub.Api.Features.Users.Wishlist;

[ApiController]
[Route("me")]
public class UserWishlistController : UserControllerBase
{
    private readonly UserWishlistService _service;

    public UserWishlistController(
        UserWishlistService service
    )
    {
        _service = service;
    }
    
    
    [HttpGet("wishlist")]
    public async Task<ActionResult<List<GameListItemDto>>> GetWishlist()
    {
        var userId = User.GetUserId();
        var result = await _service.GetWishlist(userId);

        return result.Match(
            res => Ok(res),
            err => this.ProblemErr(err));
    }

    [HttpPost("wishlist/games/{gameId}")]
    public async Task<IActionResult> AddGame(int gameId)
    {
        var userId = User.GetUserId();
        await _service.AddGameToWishlist(userId, gameId);

        return NoContent();
    }
    [HttpDelete("wishlist/games/{gameId}")]
    public async Task<IActionResult> RemoveGame(int gameId)
    {
        var userId = User.GetUserId();
        await _service.RemoveGameFromWishlist(userId, gameId);
        
        return NoContent();
    }
}