using backend.Extensions;
using ErrorOr;
using GameNewsHub.Api.Features.Shared;
using GameNewsHub.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameNewsHub.Api.Features.Users;

[ApiController]
[Route("[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly UsersService _service;
    
    public UsersController(UsersService service)
    {
        _service = service;
    }

    [HttpGet("me/games/followed")]
    public async Task<ActionResult<List<GameListItemDto>>> GetFollowedGames()
    {
        var userId = User.GetUserId();
        var result = await _service.GetFollowedGames(userId);
        return result.MatchFirst(
            details => Ok(details),
            err => this.ProblemErr(err));
    }
    [HttpGet("me/events/followed")]
    public async Task<ActionResult<List<EventListItemDto>>> GetFollowedEvents()
    {
        var userId = User.GetUserId();
        var result = await _service.GetFollowedEvents(userId);
        return result.MatchFirst(
            details => Ok(details),
            err => this.ProblemErr(err));
    }
    [HttpGet("me/genres/followed")]
    public async Task<ActionResult<List<GenreDto>>> GetFollowedGenres()
    {
        var userId = User.GetUserId();
        var result = await _service.GetFollowedGenres(userId);
        return result.MatchFirst(
            details => Ok(details),
            err => this.ProblemErr(err));
    }

    [HttpGet("me/platforms/followed")]
    public async Task<ActionResult<List<PlatformGroupDto>>> GetFollowedPlatformGroups()
    {
        var userId = User.GetUserId();
        var result = await _service.GetFollowedPlatformGroups(userId);
        return result.MatchFirst(
            details => Ok(details),
            err => this.ProblemErr(err));
    }

    [HttpGet("me/profile")]
    public async Task<ActionResult<ProfileDto>> GetProfile()
    {
        var userId = User.GetUserId();
        var result = await _service.GetFullProfile(userId);
        return result.MatchFirst(
            details => Ok(details),
            err => this.ProblemErr(err));
    }
}