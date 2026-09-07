using backend.Extensions;
using ErrorOr;
using GameNewsHub.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace GameNewsHub.Api.Features.Events;

[ApiController]
[Route("[controller]")]
public class EventController : ControllerBase
{
    private readonly EventService _service;
    private readonly FollowEventService _followService;
    
    public  EventController(EventService service, FollowEventService followService)
    {
        _service = service;
        _followService = followService;
    }

    [HttpGet]
    public async Task<ActionResult<List<EventListItemDto>>> GetEvents(DateTime? startTime, DateTime? endTime)
    {
        var result = await _service.GetEvents(startTime, endTime);
        return result;
    }
    
    [HttpGet("normalized")]
    public async Task<ActionResult<List<NormalizedEventListItemDto>>> GetNormalizedEvents(DateTime? startTime, DateTime? endTime)
    {
        var result = await _service.GetNormalizedEvents(startTime, endTime);
        return result;
    }


    [HttpGet("{eventId}")]
    public async Task<ActionResult<EventListItemDto>> GetEvent(int eventId)
    {
        var result = await _service.GetEventDetails(eventId);
        return result.Match(
            detail => Ok(detail),
            errors => Problem(errors[0].Description)
        );
    }

    [HttpPost("{eventId}/follow")]
    [Authorize]
    public async Task<IActionResult> FollowEvent(int eventId)
    {
        var userId = User.GetUserId();
        var result = await _followService.Follow(userId, eventId);
        return result.Match<IActionResult>(
            success => Ok(),
            errors => Problem(errors[0].Description));
    }
    
    [HttpDelete("{eventId}/follow")]
    [Authorize]
    public async Task<IActionResult> UnfollowEvent(int eventId)
    {
        var userId = User.GetUserId();
        var result = await _followService.Unfollow(userId, eventId);
        return result.Match<IActionResult>(
            success => Ok(),
            errors => Problem(errors[0].Description));
    }
}