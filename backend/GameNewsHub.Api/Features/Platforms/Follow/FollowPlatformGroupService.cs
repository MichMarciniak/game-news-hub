using ErrorOr;
using GameNewsHub.Data;
using Microsoft.EntityFrameworkCore;

namespace GameNewsHub.Api.Features.Platforms.Follow;

public class FollowPlatformGroupService
{
    private readonly AppDbContext _context;
    private readonly ILogger<FollowPlatformGroupService> _logger;

    public FollowPlatformGroupService(
        AppDbContext context,
        ILogger<FollowPlatformGroupService> logger
    )
    {
        _context = context;
        _logger = logger;
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