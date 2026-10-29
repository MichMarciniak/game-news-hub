using ErrorOr;
using GameNewsHub.Data;
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

        user.SteamId = result.SteamId;
        await _context.SaveChangesAsync();

        return new ErrorOr<Success>();
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
}