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
        if (!await _context.Users.AnyAsync(u => u.Id == userId))
            return Error.NotFound("User.NotFound", $"User {userId} not found.");

        var games = await _context.Users
            .Where(u => u.Id == userId)
            .SelectMany(u => u.FollowedGames)
            .ProjectToListItemDto()
            .ToListAsync();

        return games;
    }

    public async Task<ErrorOr<List<GenreDto>>> GetFollowedGenres(int userId)
    {
        if (!await _context.Users.AnyAsync(u => u.Id == userId))
            return Error.NotFound("User.NotFound", $"User {userId} not found.");

        var genres = await _context.Users
            .Where(u => u.Id == userId)
            .SelectMany(u => u.FollowedGenres)
            .ProjectToDto()
            .ToListAsync();

        return genres;
    }

    public async Task<ErrorOr<List<EventListItemDto>>> GetFollowedEvents(int userId)
    {
        if (!await _context.Users.AnyAsync(u => u.Id == userId))
            return Error.NotFound("User.NotFound", $"User {userId} not found.");

        var events = await _context.Users
            .Where(u => u.Id == userId)
            .SelectMany(u => u.FollowedEvents)
            .ProjectToListItemDto()
            .ToListAsync();

        return events;
    }

    public async Task<ErrorOr<List<PlatformGroupDto>>> GetFollowedPlatformGroups(int userId)
    {
        if (!await _context.Users.AnyAsync(u => u.Id == userId))
            return Error.NotFound("User.NotFound", $"User {userId} not found.");

        var events = await _context.Users
            .Where(u => u.Id == userId)
            .SelectMany(u => u.FollowedPlatformGroups)
            .ProjectToDto()
            .ToListAsync();

        return events;
    }
}