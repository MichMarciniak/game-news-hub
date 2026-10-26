using GameNewsHub.Api.Extensions;
using GameNewsHub.Api.Features.Shared;
using GameNewsHub.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameNewsHub.Api.Features.Games;

[ApiController]
[Route("games")]
public class GameController : ControllerBase
{
    private readonly FollowGameService _followService;
    private readonly GameService _service;

    public GameController(GameService service, FollowGameService followGameService)
    {
        _service = service;
        _followService = followGameService;
    }

    [HttpGet("search")]
    public async Task<ActionResult<SearchDto<GameListItemDto>>> SearchGames([FromQuery] GetGameSearchParams request)
    {
        var games = await _service.SearchGamesAsync(
            request.Query,
            request.Page,
            request.PageSize,
            request.SortBy,
            request.Descending 
        );
        return Ok(games);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GameDetailDto>> GetGameDetails(int id)
    {
        var result = await _service.GetGameDetailsAsync(id);
        return result.MatchFirst(
            details => Ok(details),
            err => this.ProblemErr(err));
    }

    [HttpGet("{id}/addons")]
    public async Task<ActionResult<GameDetailDto>> GetGameAddons(int id)
    {
        var result = await _service.GetGameAddons(id);
        return result.MatchFirst(
            details => Ok(details),
            err => this.ProblemErr(err));
    }

    [HttpPost("{id}/follow")]
    [Authorize]
    public async Task<IActionResult> FollowGame(int id)
    {
        var userId = User.GetUserId();
        var result = await _followService.Follow(userId, id);
        return result.MatchFirst<IActionResult>(
            success => Ok(),
            err => this.ProblemErr(err));
    }

    [HttpDelete("{id}/follow")]
    [Authorize]
    public async Task<IActionResult> UnfollowGame(int id)
    {
        var userId = User.GetUserId();
        var result = await _followService.Unfollow(userId, id);
        return result.MatchFirst<IActionResult>(
            success => NoContent(),
            err => this.ProblemErr(err));
    }

    [HttpPost("batch/follow")]
    [Authorize]
    public async Task<IActionResult> BatchFollowGames([FromBody] BatchFollowDto<int> gameIds)
    {
        var userId = User.GetUserId();
        var result = await _followService.FollowBatch(userId, gameIds.Ids);
        return result.Match<IActionResult>(
            _ => Ok(),
            err =>  this.ProblemErr(err));
    }

}