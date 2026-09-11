using ErrorOr;
using GameNewsHub.Api.Features.Shared;
using GameNewsHub.Data;
using GameNewsHub.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameNewsHub.Api.Features.Genres;

public class FollowGenreService : IFollowService<Genre>
{
    private readonly AppDbContext _context;

    public FollowGenreService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<Success>> Follow(int userId, int entityId)
    {
        var user = await _context.Users
            .Include(u => u.FollowedGenres)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return Error.NotFound("User.NotFound", $"User with id {userId} does not exist");

        var genre = await _context.Genres.FindAsync(entityId);
        if (genre == null) return Error.NotFound("Genre.NotFound", $"Genre with id {entityId} does not exist");

        if (!user.FollowedGenres.Contains(genre))
        {
            user.FollowedGenres.Add(genre);
            await _context.SaveChangesAsync();
        }

        return Result.Success;
    }

    public async Task<ErrorOr<Success>> Unfollow(int userId, int entityId)
    {
        var user = await _context.Users
            .Include(u => u.FollowedGenres)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return Error.NotFound("User.NotFound", $"User with id {userId} does not exist");

        var genre = await _context.Genres.FindAsync(entityId);
        if (genre == null) return Error.NotFound("Genre.NotFound", $"Genre with id {entityId} does not exist");

        if (user.FollowedGenres.Contains(genre))
        {
            user.FollowedGenres.Remove(genre);
            await _context.SaveChangesAsync();
        }

        return Result.Success;
    }
}