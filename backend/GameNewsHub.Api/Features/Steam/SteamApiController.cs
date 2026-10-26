using GameNewsHub.Api.Extensions;
using GameNewsHub.Api.Features.Shared;
using GameNewsHub.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GameNewsHub.Api.Features.Steam;

[ApiController]
[Route("steam")]
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
    public async Task<ActionResult<SteamGameWrapperDto>> ImportGames()
    {
        var userId = User.GetUserId();
        var result = await _steamService.ImportOwnedGames(userId);
        return result.Match(
            res => Ok(res),
            err => this.ProblemErr(err));
    }

    [HttpGet("import/wishlist")]
    public async Task<ActionResult<WishlistDto>> ImportWishlist()
    {
        var userId = User.GetUserId();
        var result = await _steamService.ImportWishlist(userId);
        return result.Match(
            res => Ok(res),
            err => this.ProblemErr(err));
    }

}