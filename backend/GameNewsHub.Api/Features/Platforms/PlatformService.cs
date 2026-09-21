using ErrorOr;
using GameNewsHub.Contracts;
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
            .Select(pg => pg.ToDto())
            .ToListAsync();

        return groups;
    }
    
    public async Task<List<PlatformGroupWithPlatformsDto>> GetPlatformGroupsFull()
    {
        var groups = await _context.PlatformGroups
            .Include(pg => pg.Platforms)
            .Select(pg => pg.ToDtoWithPlatforms())
            .ToListAsync();

        return groups;
    }

    public async Task<ErrorOr<Success>> ReassignPlatformGroup(int platformId, int groupId)
    {
        var platform = await _context.Platforms.FindAsync(platformId);

        if (platform == null)
        {
            _logger.LogWarning(
                "Reassign failed: Platform {PlatformId} not found",
                platformId);
            return Error.NotFound("Platform.NotFound",
                $"Platform {platformId} not found.");
        }

        var newGroup = await _context.PlatformGroups.FindAsync(groupId);

        if (newGroup == null)
        {
            _logger.LogWarning(
                "Reassign failed: PlatformGroup {PlatformGroupId} not found",
                groupId);
            return Error.NotFound("PlatformGroup.NotFound",
                $"Platform group {groupId} not found.");
        }

        var oldGroupId = platform.PlatformGroupId;
        platform.PlatformGroup = newGroup;
        await _context.SaveChangesAsync();
        
        _logger.LogInformation(
            "Platform {PlatformId} reassigned from group {OldPlatformGroupId} to {NewPlatformGroupId}",
            platformId, oldGroupId, groupId);

        return new ErrorOr<Success>();
    }

    public async Task<ErrorOr<Success>> FollowPlatformGroup(int userId, int groupId)
    {
        var user = await _context.Users
            .Include(u => u.FollowedPlatformGroups)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return Error.NotFound("User.NotFound",
                $"User {userId} not found");

        var group = await _context.PlatformGroups
            .FindAsync(groupId);

        if (group == null)
            return Error.NotFound("PlatformGroup.NotFound",
                $"Platform group {groupId} not found");

        if (user.FollowedPlatformGroups.Contains(group)) return new ErrorOr<Success>();

        user.FollowedPlatformGroups.Add(group);

        await _context.SaveChangesAsync();

        return new ErrorOr<Success>();
    }

    public async Task<ErrorOr<Success>> UnfollowPlatformGroup(int userId, int groupId)
    {
        var user = await _context.Users
            .Include(u => u.FollowedPlatformGroups)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return Error.NotFound("User.NotFound",
                $"User {userId} not found");

        var group = await _context.PlatformGroups
            .FindAsync(groupId);

        if (group == null)
            return Error.NotFound("PlatformGroup.NotFound",
                $"Platform group {groupId} not found");

        if (!user.FollowedPlatformGroups.Contains(group)) return new ErrorOr<Success>();

        user.FollowedPlatformGroups.Remove(group);

        await _context.SaveChangesAsync();

        return new ErrorOr<Success>();
    }
}