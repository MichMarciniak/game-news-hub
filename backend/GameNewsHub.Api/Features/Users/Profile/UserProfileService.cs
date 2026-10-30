using ErrorOr;
using GameNewsHub.Api.Features.Games;
using GameNewsHub.Api.Features.Platforms;
using GameNewsHub.Api.Mappings;
using GameNewsHub.Contracts;
using GameNewsHub.Contracts.User;
using GameNewsHub.Data;
using Microsoft.EntityFrameworkCore;

namespace GameNewsHub.Api.Features.Users.Profile;

public class UserProfileService
{
    private readonly AppDbContext _context;

    public UserProfileService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<ProfileDto>> GetProfile(int userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return Error.NotFound("User.NotFound", $"User {userId} not found");

        var settings = user.ToSettingsDto();
        return user.ToDto(false, settings);
    }

    public async Task<ErrorOr<Success>> PatchSettings(int userId, SettingsDto settings)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return Error.NotFound("User.NotFound", $"User {userId} not found.");

        user.ShowFutureRecommendations = settings.ShowFutureRecommendations;
        await _context.SaveChangesAsync();
        return new ErrorOr<Success>();
    }


    public async Task<ErrorOr<FullProfileDto>> GetFullProfile(int userId)
    {
        var user = await _context.Users
            .Include(u => u.FollowedEvents)
            .Include(u => u.FollowedGames)
            .Include(u => u.FollowedGenres)
            .Include(u => u.FollowedPlatformGroups)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return Error.NotFound("User.NotFound", $"User {userId} not found");

        var events = user.FollowedEvents
            .Select(e => e.ToListNameDto())
            .ToList();
        var games = user.FollowedGames
            .Select(g => g.ToListItemDto())
            .ToList();
        var genres = user.FollowedGenres
            .Select(g => g.ToDto())
            .ToList();
        var platformGroups = user.FollowedPlatformGroups
            .Select(pg => pg.ToDto())
            .ToList();
        var settings = user.ToSettingsDto();

        return user.ToFullDto(events, games, genres, platformGroups, settings);
    }
}