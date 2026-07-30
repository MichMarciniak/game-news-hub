using backend.Data;
using GameNewsHub.Data.Entities;
using GameNewsHub.Sync.Dtos;
using Microsoft.EntityFrameworkCore;

namespace GameNewsHub.Sync.Sync.Platforms;

public class PlatformSyncService : IPlatformSyncService
{
    private readonly AppDbContext _context;

    public PlatformSyncService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<ICollection<Platform>> GetOrCreateBatchAsync(IEnumerable<IgdbPlatformResponse> platformDtos)
    {
        if (platformDtos == null || !platformDtos.Any())
        {
            return new List<Platform>();
        }

        var ids = platformDtos.Select(p => p.Id).ToList();

        var existing = await _context.Platforms
            .Where(p => ids.Contains(p.IgdbId))
            .ToListAsync();

        var existingIds = existing.Select(p => p.IgdbId).ToHashSet();
        var missing = platformDtos.Where(p => !existingIds.Contains(p.Id)).ToList();

        if (missing.Count == 0) return existing;
        
        var newPlatforms = missing.Select(p => new Platform
        {
            Name = p.Name,
            IgdbId = p.Id
        }).ToList();
        
        _context.Platforms.AddRange(newPlatforms);
        await _context.SaveChangesAsync();
        
        existing.AddRange(newPlatforms);

        return existing;
    }
}