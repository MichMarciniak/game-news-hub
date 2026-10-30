using GameNewsHub.Api.Extensions;
using GameNewsHub.Api.Features.Shared;
using GameNewsHub.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameNewsHub.Api.Features.Users.Wishlist;

[ApiController]
[Authorize]
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
        var userId = User.GetId();
        var result = await _service.GetWishlist(userId);

        return result.ToActionResult(this);
    }

    [HttpPost("wishlist/games/{gameId}")]
    public async Task<IActionResult> PostGame(int gameId)
    {
        var userId = User.GetId();
        var result = await _service.AddGameToWishlist(userId, gameId);

        return result.ToNoContent(this);
    }
    [HttpDelete("wishlist/games/{gameId}")]
    public async Task<IActionResult> DeleteGame(int gameId)
    {
        var userId = User.GetId();
        var result = await _service.RemoveGameFromWishlist(userId, gameId);
        
        return result.ToNoContent(this);
    }
}