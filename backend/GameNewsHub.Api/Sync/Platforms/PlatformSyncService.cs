using backend.Data;
using GameNewsHub.Api.Entities;
using GameNewsHub.Api.Features.Platforms;
using GameNewsHub.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.CompilerServices;

namespace GameNewsHub.Api.Sync.Platforms;

public class PlatformSyncService : IPlatformSyncService
{
    private readonly AppDbContext _context;

    public PlatformSyncService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<ICollection<Platform>> GetOrCreateBatchAsync(IEnumerable<PlatformDto> platformDtos)
    {
        if (platformDtos == null || !platformDtos.Any())
        {
            return new List<Platform>();
        }

        var ids = platformDtos.Select(p => p.Id).ToList();

        var existing = await _context.Platforms
            .Where(p => ids.Contains(p.IgdbId))
            .ToListAsync();

        var missing = platformDtos.Except(existing.Select(p => p.ToDto()));

        if (missing.Any())
        {
            var newPlatforms = missing.Select(p => new Platform
            {
                Name = p.Name,
                IgdbId = p.Id
            }).ToList();
            
            _context.Platforms.AddRange(newPlatforms);
            await _context.SaveChangesAsync();
            
            existing.AddRange(newPlatforms);
        }

        return existing;
    }
}