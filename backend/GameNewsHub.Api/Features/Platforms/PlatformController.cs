using GameNewsHub.Contracts;
using GameNewsHub.Contracts.Platforms;
using Microsoft.AspNetCore.Mvc;

namespace GameNewsHub.Api.Features.Platforms;

[ApiController]
[Route("platforms")]
public class PlatformController : ControllerBase
{
    private readonly PlatformService _service;

    public PlatformController(PlatformService service)
    {
        _service = service;
    }

    [HttpGet("groups")]
    public async Task<ActionResult<List<PlatformGroupDto>>> GetPlatformGroups()
    {
        var result = await _service.GetPlatformGroups();

        return result;
    }

    [HttpGet("groups/full")]
    public async Task<ActionResult<List<PlatformGroupWithPlatformsDto>>> GetPlatformGroupsFull()
    {
        var result = await _service.GetPlatformGroupsFull();

        return result;
    }
}