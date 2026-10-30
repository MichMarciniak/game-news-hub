using GameNewsHub.Api.Extensions;
using GameNewsHub.Api.Features.Events;
using GameNewsHub.Api.Features.Platforms;
using GameNewsHub.Api.Features.Shared;
using GameNewsHub.Contracts;
using GameNewsHub.Contracts.Events;
using GameNewsHub.Contracts.Shared;
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
    
    // tu logger bo tu user id jest, może to przenieść do serwisów?
    private readonly ILogger<AdminController> _logger;

    public AdminController(EventService eventService, PlatformService platformService, ILogger<AdminController> logger)
    {
        _eventService = eventService;
        _platformService = platformService;
        _logger = logger;
    }
    
    [HttpPatch("events/{id}/series")]
    public async Task<IActionResult> PatchReassignSeries(int id, [FromQuery] int seriesId)
    {
        _logger.LogInformation(
            "Admin {AdminId} attempting to reassign event {EventId} to series {SeriesId}",
            User.GetId(), id, seriesId);
            
        var result = await _eventService.ReassignEventToSeries(id, seriesId);
        return result.ToNoContent(this);
    }
    
    [HttpPatch("platforms/{id}/group")]
    public async Task<IActionResult> PatchReassignPlatformGroup(int id, [FromQuery] int groupId)
    {
        _logger.LogInformation(
            "Admin {AdminId} reassigning platform {PlatformId} to group {PlatformGroupId}",
            User.GetId(), id, groupId);
        var result = await _platformService.ReassignPlatformGroup(id, groupId);

        return result.ToNoContent(this);
    }

    [HttpGet("series/search")]
    public async Task<ActionResult<SearchDto<AdminSeriesDto>>> GetSeriesSearch(
        string? query, int page = 1, int pageSize = 20)
    {
        var result = await _eventService.GetAdminSeries(query, page, pageSize);
        return result;
    }
}