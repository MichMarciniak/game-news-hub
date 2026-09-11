using GameNewsHub.Api.Extensions;
using GameNewsHub.Api.Features.Shared;
using GameNewsHub.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameNewsHub.Api.Features.Events;

[ApiController]
[Route("[controller]")]
public class EventController : ControllerBase
{
    private readonly FollowEventService _followService;
    private readonly EventService _service;

    public EventController(EventService service, FollowEventService followService)
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
    public async Task<ActionResult<List<NormalizedEventListItemDto>>> GetNormalizedEvents(DateTime? startTime,
        DateTime? endTime)
    {
        var result = await _service.GetNormalizedEvents(startTime, endTime);
        return result;
    }


    [HttpGet("{eventId}")]
    public async Task<ActionResult<EventListItemDto>> GetEvent(int eventId)
    {
        var result = await _service.GetEventDetails(eventId);
        return result.MatchFirst(
            detail => Ok(detail),
            err => this.ProblemErr(err));
    }

    [HttpGet("search")]
    public async Task<ActionResult<EventListNameDto>> SearchEvents(string query, int page = 1, int pageSize = 25)
    {
        var events = await _service.SearchEvents(query, page, pageSize);
        return Ok(events);
    }

    [HttpPost("{eventId}/follow")]
    [Authorize]
    public async Task<IActionResult> FollowEvent(int eventId)
    {
        var userId = User.GetUserId();
        var result = await _followService.Follow(userId, eventId);
        return result.MatchFirst<IActionResult>(
            success => Ok(),
            err => this.ProblemErr(err));
    }

    [HttpDelete("{eventId}/follow")]
    [Authorize]
    public async Task<IActionResult> UnfollowEvent(int eventId)
    {
        var userId = User.GetUserId();
        var result = await _followService.Unfollow(userId, eventId);
        return result.MatchFirst<IActionResult>(
            success => Ok(),
            err => this.ProblemErr(err));
    }


    [HttpPatch("reassign/series")]
    [Authorize] //admin
    public async Task<IActionResult> ReassignSeries([FromQuery] int eventId, [FromQuery] int seriesId)
    {
        var result = await _service.ReassignEventToSeries(eventId, seriesId);
        return result.MatchFirst<IActionResult>(
            success => Ok(),
            err => this.ProblemErr(err));
    }
}