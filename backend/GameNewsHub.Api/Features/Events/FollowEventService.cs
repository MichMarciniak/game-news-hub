using ErrorOr;
using GameNewsHub.Api.Features.Shared;
using GameNewsHub.Data;
using GameNewsHub.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameNewsHub.Api.Features.Events;

public class FollowEventService : IFollowService<Event>
{
    private readonly AppDbContext _context;

    public FollowEventService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<Success>> Follow(int userId, int entityId)
    {
        var user = await _context.Users
            .Include(u => u.FollowedEvents)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return Error.NotFound("User.NotFound", $"User with id {userId} does not exist");

        var e = await _context.Events.FindAsync(entityId);
        if (e == null) return Error.NotFound("Event.NotFound", $"Event with id {entityId} does not exist");

        if (!user.FollowedEvents.Contains(e))
        {
            user.FollowedEvents.Add(e);
            await _context.SaveChangesAsync();
        }

        return Result.Success;
    }

    public async Task<ErrorOr<Success>> Unfollow(int userId, int entityId)
    {
        var user = await _context.Users
            .Include(u => u.FollowedEvents)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return Error.NotFound("User.NotFound", $"User with id {userId} does not exist");

        var e = await _context.Events.FindAsync(entityId);
        if (e == null) return Error.NotFound("Event.NotFound", $"Event with id {entityId} does not exist");

        if (user.FollowedEvents.Contains(e))
        {
            user.FollowedEvents.Remove(e);
            await _context.SaveChangesAsync();
        }

        return Result.Success;
    }
}