using GameNewsHub.Api.Extensions;
using GameNewsHub.Api.Features.Shared;
using GameNewsHub.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameNewsHub.Api.Features.Users.Preferences;

[ApiController]
[Authorize]
public class UserPreferencesController : UserControllerBase 
{
    private readonly UserPreferencesService _service;

    public UserPreferencesController(
        UserPreferencesService service
    )
    {
        _service = service;
    }
    
    [HttpGet("followed-games")]
    public async Task<ActionResult<List<GameListItemDto>>> GetFollowedGames()
    {
        var userId = User.GetId();
        var result = await _service.GetFollowedGames(userId);
        return result.ToActionResult(this);
    }

    [HttpGet("followed-events")]
    public async Task<ActionResult<List<EventListItemDto>>> GetFollowedEvents()
    {
        var userId = User.GetId();
        var result = await _service.GetFollowedEvents(userId);
        return result.ToActionResult(this);
    }

    [HttpGet("followed-genres")]
    public async Task<ActionResult<List<GenreDto>>> GetFollowedGenres()
    {
        var userId = User.GetId();
        var result = await _service.GetFollowedGenres(userId);
        return result.ToActionResult(this);
    }

    [HttpGet("followed-platform-groups")]
    public async Task<ActionResult<List<PlatformGroupDto>>> GetFollowedPlatformGroups()
    {
        var userId = User.GetId();
        var result = await _service.GetFollowedPlatformGroups(userId);
        return result.ToActionResult(this);
    }
}