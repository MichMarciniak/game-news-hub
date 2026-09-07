using backend.Data;
using backend.Extensions;
using GameNewsHub.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameNewsHub.Api.Features.Genres;

[ApiController]
[Route("[controller]")]
public class GenreController : ControllerBase
{
    private readonly GenreService _service;
    private readonly FollowGenreService _followService;
    
    public GenreController(GenreService service, FollowGenreService followGenreService)
    {
        _service = service;
        _followService = followGenreService;
    }
    
    [HttpGet("genres")]
    public async Task<ActionResult<GenreDto>> GetGenres()
    {
        var genres = await _service.GetGenreListAsync();
        return Ok(genres);
    }

    [HttpPost("{genreId}/follow")]
    [Authorize]
    public async Task<IActionResult> FollowGenre(int genreId)
    {
        var userId = User.GetUserId();
        var result = await _followService.Follow(userId, genreId);
        return result.Match<IActionResult>(
            success => Ok(),
            error => Problem(error[0].Description));
    }
    
    [HttpDelete("{genreId}/follow")]
    [Authorize]
    public async Task<IActionResult> UnfollowGenre(int genreId)
    {
        var userId = User.GetUserId();
        var result = await _followService.Unfollow(userId, genreId);
        return result.Match<IActionResult>(
            success => NoContent(),
            error => Problem(error[0].Description));
    }
}