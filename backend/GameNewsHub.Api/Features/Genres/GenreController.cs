using backend.Data;
using GameNewsHub.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GameNewsHub.Api.Features.Genres;

[ApiController]
public class GenreController : ControllerBase
{
    private readonly IGenreService _service;
    
    public GenreController(IGenreService service)
    {
        _service = service;
    }
    
    [HttpGet("genres")]
    public async Task<ActionResult<GenreDto>> GetGenres()
    {
        var genres = await _service.GetGenreListAsync();
        return Ok(genres);
    }

}