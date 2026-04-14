using backend.Services.Interfaces;
using GameNewsHub.Api.Services.Background;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{

    private readonly IgdbAuthService _authService;
    private readonly IgdbClient _client;
    private readonly IGameSyncService _gameSyncService;

    public TestController(IgdbAuthService service, IgdbClient client, IGameSyncService gameSyncService)
    {
        _authService = service;
        _client = client;
        _gameSyncService = gameSyncService;
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
    [HttpGet("sync")]
    public async Task<IActionResult> SyncGames()
    {
        await _gameSyncService.SyncUpcomingGamesAsync(10);

        return Ok();
    }
    
}