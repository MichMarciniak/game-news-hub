using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace GameNewsHub.Api.Features.Games;

[Authorize]
[ApiController]
[Route("[controller]")]
public class GameController : ControllerBase
{
    private readonly IGameService _service;

    public GameController(IGameService service)
    {
        _service = service;
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetGamesList()
    {
        var games = await _service.GetGamesListAsync();
        return Ok(games);
    }
    
    [HttpGet("{gameId}")]
    public async Task<IActionResult> GetGameDetails(int gameId)
    {
        var result = await _service.GetGameDetailsAsync(gameId);
        return result.Match<IActionResult>(
            details => Ok(details),
            errors => Problem(errors[0].Description));
    }

}