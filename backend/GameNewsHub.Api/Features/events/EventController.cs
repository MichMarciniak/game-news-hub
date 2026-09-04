using ErrorOr;
using GameNewsHub.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace GameNewsHub.Api.Features.Events;

[ApiController]
[Route("[controller]")]
public class EventController : ControllerBase
{
    private readonly EventService _service;
    
    public  EventController(EventService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<EventListItemDto>>> GetEvents(DateTime? startTime, DateTime? endTime)
    {
        var result = await _service.GetEvents(startTime, endTime);
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
}