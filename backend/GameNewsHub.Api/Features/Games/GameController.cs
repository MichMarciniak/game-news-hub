using GameNewsHub.Api.Extensions;
using GameNewsHub.Api.Features.Shared;
using GameNewsHub.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace GameNewsHub.Api.Features.Games;

[ApiController]
[Route("games")]
public class GameController : ControllerBase
{
    private readonly GameService _service;

    public GameController(GameService service)
    {
        _service = service;
    }

    [HttpGet("search")]
    public async Task<ActionResult<SearchDto<GameListItemDto>>> GetGameSearch([FromQuery] GetGameSearchParams request)
    {
        var games = await _service.SearchGamesAsync(
            request.Query,
            request.Page,
            request.PageSize,
            request.SortBy,
            request.Descending 
        );

        return games;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GameDetailDto>> GetGameDetails(int id)
    {
        var result = await _service.GetGameDetailsAsync(id);
        return result.ToActionResult(this);
    }

    [HttpGet("{id}/addons")]
    public async Task<ActionResult<List<GameListItemDto>>> GetGameAddons(int id)
    {
        var result = await _service.GetGameAddons(id);
        return result;
    }
}