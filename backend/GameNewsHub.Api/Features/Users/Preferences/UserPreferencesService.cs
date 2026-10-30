using ErrorOr;
using GameNewsHub.Api.Features.Events;
using GameNewsHub.Api.Features.Games;
using GameNewsHub.Api.Features.Genres;
using GameNewsHub.Api.Features.Platforms;
using GameNewsHub.Contracts;
using GameNewsHub.Contracts.Events;
using GameNewsHub.Contracts.Games;
using GameNewsHub.Contracts.Genres;
using GameNewsHub.Contracts.Platforms;
using GameNewsHub.Data;
using Microsoft.EntityFrameworkCore;

namespace GameNewsHub.Api.Features.Users.Preferences;

public class UserPreferencesService
{
    private readonly AppDbContext _context;

    public UserPreferencesService(
        AppDbContext context
    )
    {
        _context = context;
    }
    
    public async Task<ErrorOr<List<GameListItemDto>>> GetFollowedGames(int userId)
    {
        var user = await _context.Users
            .Include(u => u.FollowedGames)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return Error.NotFound("User.NotFound", $"User {userId} not found.");

        var games = user.FollowedGames
            .Select(g => g.ToListItemDto())
            .ToList();

        return games;
    }

    public async Task<ErrorOr<List<GenreDto>>> GetFollowedGenres(int userId)
    {
        var user = await _context.Users
            .Include(u => u.FollowedGenres)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return Error.NotFound("User.NotFound", $"User {userId} not found.");

        var genres = user.FollowedGenres
            .Select(g => g.ToDto())
            .ToList();

        return genres;
    }

    public async Task<ErrorOr<List<EventListItemDto>>> GetFollowedEvents(int userId)
    {
        var user = await _context.Users
            .Include(u => u.FollowedEvents)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return Error.NotFound("User.NotFound", $"User {userId} not found.");

        var events = user.FollowedEvents
            .Select(e => e.ToListItemDto())
            .ToList();

        return events;
    }

    public async Task<ErrorOr<List<PlatformGroupDto>>> GetFollowedPlatformGroups(int userId)
    {
        var user = await _context.Users
            .Include(u => u.FollowedPlatformGroups)
            .ThenInclude(pg => pg.Platforms)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return Error.NotFound("User.NotFound", $"User {userId} not found.");

        var events = user.FollowedPlatformGroups
            .Select(pg => pg.ToDto())
            .ToList();

        return events;
    }
}