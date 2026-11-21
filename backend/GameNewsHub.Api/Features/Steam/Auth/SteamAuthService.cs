using ErrorOr;
using GameNewsHub.Data;
using GameNewsHub.Data.Entities;
using GameNewsHub.External.Steam.Client;
using Microsoft.EntityFrameworkCore;

namespace GameNewsHub.Api.Features.Steam.Auth;

public class SteamAuthService
{
    private AppDbContext _context;
    private readonly ILogger<SteamAuthService> _logger;
    private readonly SteamAuthClient _steamAuthClient;
    
    public SteamAuthService(
        AppDbContext context,
        ILogger<SteamAuthService> logger,
        SteamAuthClient steamAuthClient
    )
    {
        _context = context;
        _logger = logger;
        _steamAuthClient = steamAuthClient;
    }

    public string GetLoginUrl()
    {
        var url = _steamAuthClient.BuildLoginUrl();

        return url;
    }

    public async Task<ErrorOr<Success>> Login(int userId, IQueryCollection queryParams)
    {
        var result = await _steamAuthClient.Login(queryParams);

        if (!result.IsSuccess)
        {
            return Error.Failure(result.ErrorMessage!);
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return Error.Unauthorized("User not found");
        }

        if (!ulong.TryParse(result.SteamId, out var steamId))
        {
            return Error.Failure("Invalid steam Id");
        }

        // Sprawdź czy steamId nie jest już przypisane do innego użytkownika
        var existingProfile = await _context.Set<SteamProfile>()
            .FirstOrDefaultAsync(sp => sp.SteamId == steamId);

        if (existingProfile != null && existingProfile.AppUserId != userId)
        {
            return Error.Conflict("SteamAccountAlreadyLinked", "This Steam account is already linked to another account.");
        }

        var steamProfile = new SteamProfile
        {
            SteamId = steamId,
            AppUserId = userId,
            LastImportedAt = DateTimeOffset.UtcNow
        };

        user.SteamProfile = steamProfile;
        await _context.SaveChangesAsync();

        return new ErrorOr<Success>();
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
}