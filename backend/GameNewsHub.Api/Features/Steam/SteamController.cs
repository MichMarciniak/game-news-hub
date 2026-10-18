using GameNewsHub.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GameNewsHub.Api.Features.Steam;

[ApiController]
[Route("[controller]")]
public class SteamController : ControllerBase
{
    private readonly SteamService _service;

    public SteamController(
        SteamService service
    )
    {
        _service = service;
    }

    [HttpGet("steam-games")]
    public async Task<ActionResult<SteamGameWrapperDto>> GetGamesFromSteamProfile(string url)
    {
        var result = await _service.GetSteamGamesFromProfile(url);

        return Ok(result);
    }
    
    
    
}