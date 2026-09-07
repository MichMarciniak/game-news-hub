using backend.Extensions;
using GameNewsHub.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameNewsHub.Api.Features.Recommendations;

[Authorize]
[ApiController]
[Route("[controller]")]
public class RecommendationController : ControllerBase
{
    private readonly RecommendationService _service;

    public RecommendationController(RecommendationService service)
    {
        _service = service;
    }

    [HttpGet("events")]
    public async Task<ActionResult<List<RecommendationDto>>> GetRecommendedEvents()
    {
        var userId = User.GetUserId();
        
        var result = await _service.GetRecommendedEventList(userId);
        return Ok(result);
    }

    /*
    [HttpGet("events/newtest")]
    public async Task<ActionResult<List<RecommendationDto>>> GetNewRecommendations()
    {
        var userId = User.GetUserId();

        var result = await _service.GetNewRecommendations(userId);
        return result.Match(
            details => Ok(details),
            errors => Problem(errors[0].Description)
        );
    }
    */   
    
}