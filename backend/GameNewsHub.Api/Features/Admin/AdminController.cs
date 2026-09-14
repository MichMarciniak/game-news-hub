using GameNewsHub.Api.Extensions;
using GameNewsHub.Api.Features.Events;
using GameNewsHub.Api.Features.Platforms;
using GameNewsHub.Api.Features.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameNewsHub.Api.Features.Admin;

[ApiController]
[Route("[controller]")]
[Authorize]
public class AdminController : ControllerBase
{
    private readonly EventService _eventService;
    private readonly PlatformService _platformService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(EventService eventService, PlatformService platformService, ILogger<AdminController> logger)
    {
        _eventService = eventService;
        _platformService = platformService;
        _logger = logger;
    }
    
    
    [HttpPatch("reassign/series")]
    public async Task<IActionResult> ReassignSeries([FromQuery] int eventId, [FromQuery] int seriesId)
    {
        _logger.LogInformation(
            "Admin {AdminId} attempting to reassign event {EventId} to series {SeriesId}",
            User.GetUserId(), eventId, seriesId);
            
        var result = await _eventService.ReassignEventToSeries(eventId, seriesId);
        return result.MatchFirst<IActionResult>(
            success => Ok(),
            err => this.ProblemErr(err));
    }
    
    [HttpPatch("reassign")]
    public async Task<IActionResult> ReassignPlatformGroup([FromQuery] int platformId, [FromQuery] int groupId)
    {
        _logger.LogInformation(
            "Admin {AdminId} reassigning platform {PlatformId} to group {PlatformGroupId}",
            User.GetUserId(), platformId, groupId);
        var result = await _platformService.ReassignPlatformGroup(platformId, groupId);

        return result.MatchFirst<IActionResult>(
            success => Ok(),
            err => this.ProblemErr(err));
    }
}