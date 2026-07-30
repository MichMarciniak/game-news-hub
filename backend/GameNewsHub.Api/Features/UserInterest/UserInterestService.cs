using backend.Data;
using ErrorOr;
using GameNewsHub.Api.Features.UserInterest.Update;
using Microsoft.EntityFrameworkCore;

namespace GameNewsHub.Api.Features.UserInterest;

public class UserInterestService : IUserInterestService
{
    private readonly AppDbContext _context;
    private readonly IWeightUpdateQueue _queue;
    
    public UserInterestService(AppDbContext context, IWeightUpdateQueue queue)
    {
        _context = context;
        _queue = queue;
    }
    
    public async Task<ErrorOr<Success>> ToggleFollowGameAsync(int gameId, int userId)
    {
        var user = await _context.Users
            .Include(u => u.FollowedGames)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) {
            return Error.NotFound("User.NotFound", $"User with id {userId} not found.");
        }

        var game = await _context.Games.FindAsync(gameId);
        if (game == null) {
            return Error.NotFound("Game.NotFound", $"Game with id {gameId} not found.");
        }

        if (user.FollowedGames.Contains(game)) {
            user.FollowedGames.Remove(game);
        }
        else
        {
            user.FollowedGames.Add(game);
        }
        // change user interest
        await _context.SaveChangesAsync();
        
        await _queue.QueueUpdateAsync(userId);
        
        return Result.Success;
    }

    public async Task<ErrorOr<Success>> ToggleFollowGenreAsync(int genreId, int userId)
    {
        var user = await _context.Users
            .Include(u => u.FollowedGenres)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) {
            return Error.NotFound("User.NotFound", $"User with id {userId} not found.");
        }

        var genre = await _context.Genres.FindAsync(genreId);
        if (genre == null) {
            return Error.NotFound("Genre.NotFound", $"Genre with id {genreId} not found.");
        }
        
        if (user.FollowedGenres.Contains(genre)) {
            user.FollowedGenres.Remove(genre);
        }
        else
        {
            user.FollowedGenres.Add(genre);
        }
        // change user interest
        await _context.SaveChangesAsync();

        await _queue.QueueUpdateAsync(userId);
        
        return Result.Success;
    }

    public async Task<ErrorOr<Success>> ToggleFollowEventAsync(int eventId, int userId)
    {
        var user = await _context.Users
            .Include(u => u.FollowedEvents)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) {
            return Error.NotFound("User.NotFound", $"User with id {userId} not found.");
        }

        var ev = await _context.Events.FindAsync(eventId);
        if (ev == null) {
            return Error.NotFound("Event.NotFound", $"Event with id {eventId} not found.");
        }

        if (user.FollowedEvents.Contains(ev))
        {
            user.FollowedEvents.Remove(ev);
        }
        else
        {
            user.FollowedEvents.Add(ev);
        }

        await _context.SaveChangesAsync();
        await _queue.QueueUpdateAsync(userId);

        return Result.Success;
    }

    public async Task<ErrorOr<List<int>>> GetFollowedGamesAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.FollowedGames)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return Error.NotFound("User.NotFound", $"User with id {userId} not found.");
        }
        
        var followedGameIds = user.FollowedGames.Select(g => g.Id).ToList();
        return followedGameIds;
    }

    public async Task<ErrorOr<List<int>>> GetFollowedGenresAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.FollowedGenres)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return Error.NotFound("User.NotFound", $"User with id {userId} not found.");
        }
        
        var followedGenreIds = user.FollowedGenres.Select(g => g.Id).ToList();
        return followedGenreIds;
    }
}