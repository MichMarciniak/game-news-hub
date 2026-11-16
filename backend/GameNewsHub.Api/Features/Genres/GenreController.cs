using GameNewsHub.Contracts;
using GameNewsHub.Contracts.Genres;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace GameNewsHub.Api.Features.Genres;

[ApiController]
[Route("genres")]
[EnableRateLimiting("ReadLenient")]
public class GenreController : ControllerBase
{
    private readonly GenreService _service;

    public GenreController(GenreService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<GenreDto>>> GetGenres()
    {
        var genres = await _service.GetGenreListAsync();
        return Ok(genres);
    }

}