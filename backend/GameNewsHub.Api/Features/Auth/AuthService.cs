using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using backend.Configuration;
using ErrorOr;
using GameNewsHub.Contracts;
using GameNewsHub.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;

namespace GameNewsHub.Api.Features.Auth;

public class AuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IEmailSender _emailSender;
    private readonly FrontendOptions _frontendOptions;
    private readonly TokenService _tokenService;

    public AuthService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailSender emailSender, TokenService tokenService, IOptions<FrontendOptions> frontendOptions)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
        _tokenService = tokenService;
        _frontendOptions = frontendOptions.Value;
    }

    // tworzy usera
    // wysyła maila
    public async Task<ErrorOr<Success>> Register(RegisterDto registerDto)
    {
        var user = new AppUser
        {
            Email = registerDto.Email,
            UserName = registerDto.Username,
        };
        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
        {
            return result.Errors
                .Select(e => Error.Validation(code: e.Code, description: e.Description))
                .ToList();
        }

        await SendConfirmationEmail(user);
        
        return new ErrorOr<Success>();
    }

    // zwraca access i refresh
    public async Task<ErrorOr<TokenResult>> Login(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null)
        {
            return Error.Unauthorized(code: "Auth.InvalidCredentials", description: "Invalid email or password");
        }

        var signInResult = await _signInManager.CheckPasswordSignInAsync(
            user, loginDto.Password, true);

        if (signInResult.IsLockedOut)
        {
            var remainingMinutes = user.LockoutEnd.HasValue
                ? Math.Max(0, (int)(user.LockoutEnd.Value - DateTimeOffset.UtcNow).TotalMinutes)
                : 0;
            
            return Error.Unauthorized(code: "Auth.LockedOut",
                description: "Account temporarily locked out after subsequent failed login attempts",
                metadata: new Dictionary<string, object> {["retryAfterMinutes"] = remainingMinutes}
            );
        }
        
        if (signInResult.IsNotAllowed)
        {
            return Error.Unauthorized(code: "Auth.InvalidCredentials", description: "Invalid email or password");
        }

        
        if (!user.EmailConfirmed)
        {
            return Error.Unauthorized(code: "Auth.EmailNotConfirmed", description: "Confirm email before logging in");
        }

        var accessToken = await _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken(user);
        
        return new TokenResult {AccessToken = accessToken, RefreshToken = refreshToken}; 
    }

    // usuwa access i refresh token
    // ustawia nowy stamp w userze
    public async Task<ErrorOr<Success>> LogoutFull(int userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return Result.Success;

        await _userManager.UpdateSecurityStampAsync(user);

        return Result.Success;
    }

    // ustawia emailVerified na true
    // jeśli token i userId sie zgadzaja
    public async Task<ErrorOr<Success>> ConfirmEmail(int userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return Error.NotFound("User.NotFound", $"User does not exist");
        }

        if (user.EmailConfirmed)
        {
            return Result.Success;
        }

        IdentityResult result;
        try
        {
            result = await _userManager.ConfirmEmailAsync(user, token);
        }
        catch (FormatException)
        {
            return Error.Validation(code: "Token.Invalid", description: "Invalid token");
        }

        if (!result.Succeeded)
        {
            return result.Errors
                .Select(e => Error.Validation(code: e.Code, description: e.Description))
                .ToList();
        }

        return Result.Success;
    }
    
    public async Task<ErrorOr<TokenResult>> RefreshToken(string refreshToken)
    {
        var principal = _tokenService.ValidateRefreshToken(refreshToken);
        if (principal == null)
        {
            return Error.Unauthorized(code: "Auth.InvalidRefreshToken", description: "Refresh token is invalid");
        }

        var userId = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (userId == null)
        {
            return Error.Unauthorized(code: "Auth.InvalidRefreshToken", description: "Refresh token is invalid");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Error.Unauthorized(code: "Auth.InvalidRefreshToken", description: "Refresh token is invalid");
        }

        var stampInToken = principal.FindFirstValue("security_stamp");
        if (stampInToken != user.SecurityStamp)
        {
            return Error.Unauthorized(code: "Auth.TokenRevoked", description: "Session expired. Log in again");
        }

        var newAccessToken = await _tokenService.GenerateAccessToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken(user); // rotacja - nowy refresh przy kazdym odswiezeniu

        return new TokenResult { AccessToken = newAccessToken, RefreshToken = newRefreshToken };
    }
    
    
    public async Task<ErrorOr<Success>> ResendConfirmation(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null || user.EmailConfirmed)
        {
            return Result.Success;
        }

        await SendConfirmationEmail(user);

        return Result.Success;
    }

    private async Task SendConfirmationEmail(AppUser user)
    {
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = WebUtility.UrlEncode(token);
        
        var confirmUrl = $"{_frontendOptions.Url}/confirm-email?userId={user.Id}&token={encodedToken}";

        // powinien juz być email w userze
        await _emailSender.SendEmailAsync(user.Email,
            "Confirm your email",
            $"<p>Click to confirm <a href={confirmUrl}>{confirmUrl}</a> </p>"
        );
        
    }
    
    
}