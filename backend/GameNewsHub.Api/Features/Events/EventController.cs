using GameNewsHub.Api.Extensions;
using GameNewsHub.Api.Features.Shared;
using GameNewsHub.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GameNewsHub.Api.Features.Events;

[ApiController]
[Route("events")]
public class EventController : ControllerBase
{
    private readonly EventService _service;

    public EventController(EventService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<EventListItemDto>>> GetEvents(DateTimeOffset? from, DateTimeOffset? to)
    {
        var result = await _service.GetEvents(from, to);
        return result;
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<EventDetailDto>> GetEvent(int id)
    {
        var result = await _service.GetEventDetails(id);
        return result.ToActionResult(this);
    }

    [HttpGet("search")]
    public async Task<ActionResult<SearchDto<EventListItemDto>>> GetEventSearch(
        string? query, int page = 1, int pageSize = 25,
        EventSortBy sortBy = EventSortBy.Relevance, bool descending = true)
    {
        var events = await _service.SearchEvents(query, page, pageSize, sortBy, descending);
        return events;
    }


}