using ErrorOr;
using GameNewsHub.Api.Features.Events;
using GameNewsHub.Api.Features.Games;
using GameNewsHub.Api.Features.Genres;
using GameNewsHub.Api.Features.Platforms;
using GameNewsHub.Contracts;
using GameNewsHub.Contracts.User;
using GameNewsHub.Data;
using GameNewsHub.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GameNewsHub.Api.Features.Users.Profile;

public class UserProfileService
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public UserProfileService(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<ErrorOr<ProfileDto>> GetProfile(int userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return Error.NotFound("User.NotFound", $"User {userId} not found");

        var settings = ProfileMapper.ToSettingsDto(user);
        var result = ProfileMapper.ToDto(user);
        result.Settings = settings;
        result.IsAdmin = false;
        result.IsGoogleConnected = (await _userManager.GetLoginsAsync(user))
            .Any(l => l.LoginProvider == "Google");
        return result;
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
            .AsNoTracking()
            .AsSplitQuery()
            .Include(u => u.FollowedEvents)
            .Include(u => u.FollowedGames)
            .Include(u => u.FollowedGenres)
            .Include(u => u.FollowedPlatformGroups)
            .Include(u => u.WishlistedGames)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return Error.NotFound("User.NotFound", $"User {userId} not found");

        var settings = ProfileMapper.ToSettingsDto(user);
        var result = ProfileMapper.ToFullDto(user);
        result.Settings = settings;
        result.IsGoogleConnected = (await _userManager.GetLoginsAsync(user))
            .Any(l => l.LoginProvider == "Google");
        return result;
    }
}