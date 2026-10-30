using GameNewsHub.Api.Extensions;
using GameNewsHub.Api.Features.Shared;
using GameNewsHub.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameNewsHub.Api.Features.Steam;

[ApiController]
[Route("steam")]
[Authorize]
public class SteamApiController : ControllerBase
{
    private readonly SteamService _steamService;

    public SteamApiController(
        SteamService steamService
    )
    {
        _steamService = steamService;
    }

    [HttpGet("import/games")]
    public async Task<ActionResult<SteamGameWrapperDto>> GetGamesFromSteam()
    {
        var userId = User.GetId();
        var result = await _steamService.ImportOwnedGames(userId);
        return result.ToActionResult(this);
    }

    [HttpGet("import/wishlist")]
    public async Task<ActionResult<WishlistDto>> GetWishlistFromSteam()
    {
        var userId = User.GetId();
        var result = await _steamService.ImportWishlist(userId);
        return result.ToActionResult(this);
    }

}