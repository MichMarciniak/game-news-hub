using GameNewsHub.Api.External;
using GameNewsHub.Api.Sync.Events;
using GameNewsHub.Api.Sync.Games;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameNewsHub.Api.Features;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{

    private readonly IgdbAuthService _authService;
    private readonly IIgdbClient _client;
    private readonly IGameSyncService _gameSyncService;
    private readonly IEventSyncService _eventSyncService;

    public TestController(IgdbAuthService service, IIgdbClient client, IGameSyncService gameSyncService, IEventSyncService eventSyncService)
    {
        _authService = service;
        _client = client;
        _gameSyncService = gameSyncService;
        _eventSyncService = eventSyncService;
    }
    
    [Authorize]
    [HttpGet]
    public IActionResult Test()
    {
        return Ok();
    }

    [Authorize]
    [HttpGet("twitch")]
    public async Task<IActionResult> GetTwitchToken()
    {
        var token = await _authService.GetAccessTokenAsync();
        return Ok(new
        {
            message = "This is just for testing. Delete it later",
            token = token,
        });
    }

    [Authorize]
    [HttpGet("games")]
    public async Task<IActionResult> GetGames()
    {
        var games = await _client.GetGamesFromIgdb();

        return Ok(games);
    }

    [Authorize]
    [HttpGet("games/sync")]
    public async Task<IActionResult> SyncGames()
    {
        await _gameSyncService.SyncUpcomingGamesAsync(10);

        return Ok();
    }
    
    [Authorize]
    [HttpGet("events/sync")]
    public async Task<IActionResult> SyncEvents()
    {
        await _eventSyncService.DiscoverNewEventsAsync();

        await _eventSyncService.HydrateEventsAsync();

        return Ok();
    }
    
}