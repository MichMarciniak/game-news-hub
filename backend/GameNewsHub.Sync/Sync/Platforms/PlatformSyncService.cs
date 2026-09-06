using backend.Data;
using Data.Entities;
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

        var ids = platformDtos.Select(p => p.IgdbId).ToList();

        var existing = await _context.Platforms
            .Where(p => ids.Contains(p.IgdbId))
            .ToListAsync();

        var existingIds = existing.Select(p => p.IgdbId).ToHashSet();
        var missing = platformDtos.Where(p => !existingIds.Contains(p.IgdbId)).ToList();

        if (missing.Count == 0) return existing;

        var groups = await _context.PlatformGroups.ToListAsync();

        var newPlatforms = missing.Select(p => new Platform
        {
            Name = p.Name,
            IgdbId = p.IgdbId,
            PlatformGroup = GuessGroup(p.Name, groups)
        }).ToList();

        _context.Platforms.AddRange(newPlatforms);
        await _context.SaveChangesAsync();

        existing.AddRange(newPlatforms);

        return existing;
    }

    private PlatformGroup GuessGroup(string name, List<PlatformGroup> groups)
    {
        var n = name.ToLowerInvariant();
        
        string? targetGroupName = n switch
        {
            _ when n.Contains("playstation") || n.Contains("ps vista") => "PlayStation",
            _ when n.Contains("xbox") => "Xbox",
            _ when n.Contains("nintendo") || n.Contains("switch")
                                          || n.Contains("wii") || n.Contains("game boy")
                                          || n.Contains("3ds") || n.Contains("nes") => "Nintendo",
            _ when n.Contains("pc") || n.Contains("windows")
                                    || n.Contains("linux") || n.Contains("mac") => "PC",
            _ when n.Contains("ios") || n.Contains("android") => "Mobile",
            _ => null
        };

        var fallbackGroupName = targetGroupName ?? "Other";

        var group = groups.FirstOrDefault(g => g.Name == fallbackGroupName)
                    ?? groups.First(g => g.Name == "Other");

        return group; 
    }
}