using System.Security.Claims;
using GameNewsHub.Api.Extensions;
using GameNewsHub.Contracts.Recommendations;
using GameNewsHub.Recommendation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameNewsHub.Api.Features.Recommendations;

[ApiController]
[Route("[controller]")]
[Authorize]
public class RecommendationController : ControllerBase
{
    private readonly RecommendationService _service;

    public RecommendationController(RecommendationService service)
    {
        _service = service;
    }


    [HttpGet("events")]
    [AllowAnonymous]
    public async Task<ActionResult<RecommendationResponse>> GetEvents(
        [FromQuery] DateTimeOffset? from,
        [FromQuery] DateTimeOffset? to,
        CancellationToken ct = default)
    {
        int? userId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

        return Ok(await _service.GetRecommendationsAsync(userId, from, to, ct));
    }

}