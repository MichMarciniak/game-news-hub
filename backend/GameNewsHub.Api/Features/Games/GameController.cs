using backend.Extensions;
using ErrorOr;
using GameNewsHub.Api.Features.Shared;
using GameNewsHub.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace GameNewsHub.Api.Features.Games;

[ApiController]
[Route("[controller]")]
public class GameController : ControllerBase 
{
    private readonly GameService _service;
    private readonly FollowGameService _followService;

    public GameController(GameService service, FollowGameService followGameService)
    {
        _service = service;
        _followService = followGameService;
    }

    [HttpGet("list")]
    public async Task<ActionResult<List<GameListItemDto>>> GetGamesList()
    {
        var games = await _service.GetGamesListAsync();
        return Ok(games);
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<GameListItemDto>>> SearchGames(string? query, int page = 1, int pageSize = 25)
    {
        var games = await _service.SearchGamesAsync(query, page, pageSize);
        return Ok(games);
    }
    
    [HttpGet("{gameId}")]
    public async Task<ActionResult<GameDetailDto>> GetGameDetails(int gameId)
    {
        var result = await _service.GetGameDetailsAsync(gameId);
        return result.MatchFirst(
            details => Ok(details),
            err => this.ProblemErr(err));
    }

    [HttpGet("{gameId}/addons")]
    public async Task<ActionResult<GameDetailDto>> GetGameAddons(int gameId)
    {
        var result = await _service.GetGameAddons(gameId);
        return result.MatchFirst(
            details => Ok(details),
            err => this.ProblemErr(err));
    }

    [HttpPost("{gameId}/follow")]
    [Authorize]
    public async Task<IActionResult> FollowGame(int gameId)
    {
        int userId = User.GetUserId();
        var result = await _followService.Follow(userId, gameId);
        return result.MatchFirst<IActionResult>(
            success => Ok(),
            err => this.ProblemErr(err));
    }

    [HttpDelete("{gameId}/follow")]
    [Authorize]
    public async Task<IActionResult> UnfollowGame(int gameId)
    {
        int userId = User.GetUserId();
        var result = await _followService.Unfollow(userId, gameId);
        return result.MatchFirst<IActionResult>(
            success => NoContent(),
            err => this.ProblemErr(err));
    }

}