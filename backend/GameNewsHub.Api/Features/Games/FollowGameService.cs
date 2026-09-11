using ErrorOr;
using GameNewsHub.Api.Features.Shared;
using GameNewsHub.Data;
using GameNewsHub.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameNewsHub.Api.Features.Games;

public class FollowGameService : IFollowService<Game>
{
    private readonly AppDbContext _context;
    private readonly ILogger<FollowGameService> _logger;

    public FollowGameService(AppDbContext context, ILogger<FollowGameService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ErrorOr<Success>> Follow(int userId, int entityId)
    {
        var user = await _context.Users
            .Include(u => u.FollowedGames)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return Error.NotFound("User.NotFound", $"User with id {userId} does not exist");

        var game = await _context.Games.FindAsync(entityId);
        if (game == null) return Error.NotFound("Game.NotFound", $"Game with id {entityId} does not exist");

        if (!user.FollowedGames.Contains(game))
        {
            user.FollowedGames.Add(game);
            await _context.SaveChangesAsync();
        }

        return Result.Success;
    }

    public async Task<ErrorOr<Success>> Unfollow(int userId, int entityId)
    {
        var user = await _context.Users
            .Include(u => u.FollowedGames)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return Error.NotFound("User.NotFound", $"User with id {userId} does not exist");

        var game = await _context.Games.FindAsync(entityId);
        if (game == null) return Error.NotFound("Game.NotFound", $"Game with id {entityId} does not exist");

        if (user.FollowedGames.Contains(game))
        {
            user.FollowedGames.Remove(game);
            await _context.SaveChangesAsync();
        }

        return Result.Success;
    }
}