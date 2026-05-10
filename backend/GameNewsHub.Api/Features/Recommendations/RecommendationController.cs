using System.Security.Claims;
using backend.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameNewsHub.Api.Features.Recommendations;

[Authorize]
[ApiController]
[Route("[controller]")]
public class RecommendationController : ControllerBase
{
    private readonly IRecommendationService _service;

    public RecommendationController(IRecommendationService service)
    {
        _service = service;
    }

    [HttpGet("events")]
    public async Task<IActionResult> GetRecommendedEvents()
    {
        var userId = User.GetUserId();
        
        var result = await _service.GetRecommendedEventList(userId);
        return Ok(result);
    }
    
    
}