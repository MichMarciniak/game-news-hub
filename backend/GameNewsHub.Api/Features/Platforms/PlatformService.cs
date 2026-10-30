using ErrorOr;
using GameNewsHub.Contracts;
using GameNewsHub.Contracts.Platforms;
using GameNewsHub.Data;
using Microsoft.EntityFrameworkCore;

namespace GameNewsHub.Api.Features.Platforms;

public class PlatformService
{
    private readonly AppDbContext _context;
    private readonly ILogger<PlatformService> _logger;

    public PlatformService(AppDbContext context, ILogger<PlatformService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<PlatformGroupDto>> GetPlatformGroups()
    {
        var groups = await _context.PlatformGroups
            .Include(pg => pg.Platforms)
            .ProjectToDto()
            .ToListAsync();

        return groups;
    }

    public async Task<List<PlatformGroupWithPlatformsDto>> GetPlatformGroupsFull()
    {
        var groups = await _context.PlatformGroups
            .Include(pg => pg.Platforms)
            .ToListAsync();

        return groups
            .Select(group =>
            {
                var platforms = group.Platforms
                    .Select(PlatformMapper.ToDto)
                    .ToList();

                var dto = PlatformMapper.ToDtoWithPlatforms(group);
                dto.Platforms = platforms;
                return dto;
            })
            .ToList();
    }

}