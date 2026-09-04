using backend.Extensions;
using ErrorOr;
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
        return result.Match(
            details => Ok(details),
            errors => Problem(errors[0].Description));
    }
    [HttpGet("me/events/followed")]
    public async Task<ActionResult<List<GameListItemDto>>> GetFollowedEvents()
    {
        var userId = User.GetUserId();
        var result = await _service.GetFollowedEvents(userId);
        return result.Match(
            details => Ok(details),
            errors => Problem(errors[0].Description));
    }
    [HttpGet("me/genres/followed")]
    public async Task<ActionResult<List<GameListItemDto>>> GetFollowedGenres()
    {
        var userId = User.GetUserId();
        var result = await _service.GetFollowedGenres(userId);
        return result.Match(
            details => Ok(details),
            errors => Problem(errors[0].Description));
    }
}