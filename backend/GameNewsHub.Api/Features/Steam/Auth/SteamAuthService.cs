using ErrorOr;
using GameNewsHub.Data;
using GameNewsHub.Data.Entities;
using GameNewsHub.External.Steam.Client;

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

        var steamProfile = new SteamProfile
        {
            SteamId = steamId
        };

        user.SteamProfile = steamProfile;
        await _context.SaveChangesAsync();

        return new ErrorOr<Success>();
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
}