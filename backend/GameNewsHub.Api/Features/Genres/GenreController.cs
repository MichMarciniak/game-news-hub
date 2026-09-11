using GameNewsHub.Api.Extensions;
using GameNewsHub.Api.Features.Shared;
using GameNewsHub.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameNewsHub.Api.Features.Genres;

[ApiController]
[Route("[controller]")]
public class GenreController : ControllerBase
{
    private readonly FollowGenreService _followService;
    private readonly GenreService _service;

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
        return result.MatchFirst<IActionResult>(
            success => Ok(),
            err => this.ProblemErr(err));
    }

    [HttpDelete("{genreId}/follow")]
    [Authorize]
    public async Task<IActionResult> UnfollowGenre(int genreId)
    {
        var userId = User.GetUserId();
        var result = await _followService.Unfollow(userId, genreId);
        return result.MatchFirst<IActionResult>(
            success => NoContent(),
            err => this.ProblemErr(err));
    }
}