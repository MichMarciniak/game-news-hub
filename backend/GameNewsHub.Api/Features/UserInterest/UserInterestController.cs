using backend.Extensions;
using GameNewsHub.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameNewsHub.Api.Features.UserInterest;

[ApiController]
[Authorize]
public class UserInterestController : ControllerBase
{
    private readonly IUserInterestService _service;

    public UserInterestController(IUserInterestService service)
    {
        _service = service;
    }

    [HttpPost("follow/game/{gameId}")]
    public async Task<IActionResult> ToggleFollowGame(int gameId)
    {
        var userId = User.GetUserId();
        var result = await _service.ToggleFollowGameAsync(gameId, userId);
        return result.Match<IActionResult>(
            success => Ok(),
            errors => Problem(errors[0].Description));
    }
    
    [HttpPost("follow/genre/{genreId}")]
    public async Task<IActionResult> ToggleFollowGenre(int genreId)
    {
        var userId = User.GetUserId();
        var result = await _service.ToggleFollowGenreAsync(genreId, userId);
        return result.Match<IActionResult>(
            success => Ok(),
            errors => Problem(errors[0].Description));
    }

    [HttpPost("follow/event/{eventId}")]
    public async Task<IActionResult> ToggleFollowEvent(int eventId)
    {
        var userId = User.GetUserId();
        var result = await _service.ToggleFollowEventAsync(eventId, userId);
        return result.Match<IActionResult>(
            success => Ok(),
            errors => Problem(errors[0].Description));
    }

    [HttpGet("followed/games")]
    public async Task<ActionResult<List<int>>> GetFollowedGames()
    {
        var userId = User.GetUserId();
        var result = await _service.GetFollowedGamesAsync(userId);
        return result.Match(
            games => Ok(games),
            errors => Problem(errors[0].Description));
    }
    
    [HttpGet("followed/genres")]
    public async Task<ActionResult<List<int>>> GetFollowedGenres()
    {
        var userId = User.GetUserId();
        var result = await _service.GetFollowedGenresAsync(userId);
        return result.Match(
            genres => Ok(genres),
            errors => Problem(errors[0].Description));
    }
}