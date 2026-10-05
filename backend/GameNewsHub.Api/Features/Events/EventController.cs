using GameNewsHub.Api.Extensions;
using GameNewsHub.Api.Features.Shared;
using GameNewsHub.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameNewsHub.Api.Features.Events;

[ApiController]
[Route("events")]
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
    public async Task<ActionResult<List<EventListItemDto>>> GetEvents(DateTimeOffset? from, DateTimeOffset? to)
    {
        var result = await _service.GetEvents(from, to);
        return result;
    }

    [HttpGet("normalized")]
    public async Task<ActionResult<List<NormalizedEventListItemDto>>> GetNormalizedEvents(DateTime? startTime,
        DateTime? endTime)
    {
        var result = await _service.GetNormalizedEvents(startTime, endTime);
        return result;
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<EventDetailDto>> GetEvent(int id)
    {
        var result = await _service.GetEventDetails(id);
        return result.MatchFirst(
            detail => Ok(detail),
            err => this.ProblemErr(err));
    }

    [HttpGet("search")]
    public async Task<ActionResult<SearchDto<EventListItemDto>>> SearchEvents(
        string? query, int page = 1, int pageSize = 25,
        EventSortBy sortBy = EventSortBy.Relevance, bool descending = true)
    {
        var events = await _service.SearchEvents(query, page, pageSize, sortBy, descending);
        return Ok(events);
    }

    [HttpPost("{id}/follow")]
    [Authorize]
    public async Task<IActionResult> FollowEvent(int id)
    {
        var userId = User.GetUserId();
        var result = await _followService.Follow(userId, id);
        return result.MatchFirst<IActionResult>(
            success => Ok(),
            err => this.ProblemErr(err));
    }

    [HttpDelete("{id}/follow")]
    [Authorize]
    public async Task<IActionResult> UnfollowEvent(int id)
    {
        var userId = User.GetUserId();
        var result = await _followService.Unfollow(userId, id);
        return result.MatchFirst<IActionResult>(
            success => Ok(),
            err => this.ProblemErr(err));
    }

}