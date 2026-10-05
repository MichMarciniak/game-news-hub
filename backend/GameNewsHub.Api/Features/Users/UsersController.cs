using GameNewsHub.Api.Extensions;
using GameNewsHub.Api.Features.Shared;
using GameNewsHub.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameNewsHub.Api.Features.Users;

[ApiController]
[Route("users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly UsersService _service;

    public UsersController(UsersService service)
    {
        _service = service;
    }

    [HttpPatch("me/settings")]
    public async Task<IActionResult> PatchSettings(SettingsDto settings)
    {
        var userId = User.GetUserId();
        var result = await _service.PatchSettings(userId, settings);
        return result.Match(
            details => Ok(details),
            err => this.ProblemErr(err));
            
    }

    [HttpGet("me/followed-games")]
    public async Task<ActionResult<List<GameListItemDto>>> GetFollowedGames()
    {
        var userId = User.GetUserId();
        var result = await _service.GetFollowedGames(userId);
        return result.MatchFirst(
            details => Ok(details),
            err => this.ProblemErr(err));
    }

    [HttpGet("me/followed-events")]
    public async Task<ActionResult<List<EventListItemDto>>> GetFollowedEvents()
    {
        var userId = User.GetUserId();
        var result = await _service.GetFollowedEvents(userId);
        return result.MatchFirst(
            details => Ok(details),
            err => this.ProblemErr(err));
    }

    [HttpGet("me/followed-genres")]
    public async Task<ActionResult<List<GenreDto>>> GetFollowedGenres()
    {
        var userId = User.GetUserId();
        var result = await _service.GetFollowedGenres(userId);
        return result.MatchFirst(
            details => Ok(details),
            err => this.ProblemErr(err));
    }

    [HttpGet("me/followed-platform-groups")]
    public async Task<ActionResult<List<PlatformGroupDto>>> GetFollowedPlatformGroups()
    {
        var userId = User.GetUserId();
        var result = await _service.GetFollowedPlatformGroups(userId);
        return result.MatchFirst(
            details => Ok(details),
            err => this.ProblemErr(err));
    }

    [HttpGet("me")]
    public async Task<ActionResult<ProfileDto>> GetProfile()
    {
        var userId = User.GetUserId();
        var result = await _service.GetFullProfile(userId);
        return result.MatchFirst(
            details => Ok(details),
            err => this.ProblemErr(err));
    }
    
    [HttpGet("me/full")]
    public async Task<ActionResult<ProfileDto>> GetFullProfile()
    {
        var userId = User.GetUserId();
        var result = await _service.GetFullProfile(userId);
        return result.MatchFirst(
            details => Ok(details),
            err => this.ProblemErr(err));
    }
}