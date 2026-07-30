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
    public async Task<ActionResult<List<GameListResponse>>> GetGamesList()
    {
        var games = await _service.GetGamesListAsync();
        return Ok(games);
    }
    
    [HttpGet("{gameId}")]
    public async Task<ActionResult<GameDetailsResponse>> GetGameDetails(int gameId)
    {
        var result = await _service.GetGameDetailsAsync(gameId);
        return result.Match(
            details => Ok(details),
            errors => Problem(errors[0].Description));
    }

}