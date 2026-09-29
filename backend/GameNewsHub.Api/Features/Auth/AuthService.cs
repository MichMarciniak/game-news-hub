using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using ErrorOr;
using GameNewsHub.Api.Constants;
using GameNewsHub.Api.Features.Shared.Email;
using GameNewsHub.Api.Mappings;
using GameNewsHub.Api.Options;
using GameNewsHub.Contracts;
using GameNewsHub.Data.Constants;
using GameNewsHub.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;

namespace GameNewsHub.Api.Features.Auth;

public class AuthService
{
    private readonly IEmailSender _emailSender;
    private readonly FrontendOptions _frontendOptions;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly EmailTemplateService _templateService;
    private readonly TokenService _tokenService;
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<AuthService> _logger;

    public AuthService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailSender emailSender,
        TokenService tokenService, IOptions<FrontendOptions> frontendOptions, EmailTemplateService templateService,
        RoleManager<IdentityRole<int>> roleManager, ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
        _tokenService = tokenService;
        _frontendOptions = frontendOptions.Value;
        _templateService = templateService;
        _roleManager = roleManager;
        _logger = logger;
    }

    // tworzy usera
    // wysyła maila
    public async Task<ErrorOr<EmailDto>> Register(RegisterDto registerDto)
    {
        var user = new AppUser
        {
            Email = registerDto.Email,
            UserName = registerDto.Username,
            CreatedAt = DateTimeOffset.UtcNow
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
        {
            _logger.LogWarning(
                "Register failed for {Email}: {Errors}",
                registerDto.Email,
                string.Join(",", result.Errors.Select(e => e.Code)));
            return result.Errors
                .Select(e => Error.Validation(e.Code, e.Description))
                .ToList();
        }

        await AssignRoleAsync(user, Roles.User);
        await SendConfirmationEmail(user);
        
        _logger.LogInformation(
            "User {UserId} registered with email {EmailId}",
            user.Id, user.Email);

        return new EmailDto
        {
            Email = user.Email
        };
    }

    // zwraca access i refresh
    public async Task<ErrorOr<LoginResult>> Login(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null)
        {
            _logger.LogWarning(
                "Login failed: unknown email {Email}",
                loginDto.Email);
            return Error.Unauthorized("Auth.InvalidCredentials", "Invalid email or password");
        }

        var signInResult = await _signInManager.CheckPasswordSignInAsync(
            user, loginDto.Password, true);

        if (signInResult.IsLockedOut)
        {
            _logger.LogWarning(
                "Login blocked: User {UserId} is locked out",
                user.Id);
            
            var remainingMinutes = user.LockoutEnd.HasValue
                ? Math.Max(0, (int)(user.LockoutEnd.Value - DateTimeOffset.UtcNow).TotalMinutes)
                : 0;

            return Error.Unauthorized("Auth.LockedOut",
                "Account temporarily locked out after subsequent failed login attempts",
                new Dictionary<string, object> { ["retryAfterMinutes"] = remainingMinutes }
            );
        }

        if (!signInResult.Succeeded)
        {
            _logger.LogWarning(
                "Login failed: invalid password for User {UserId}",
                user.Id);
            return Error.Unauthorized("Auth.InvalidCredentials", "Invalid email or password");
        }


        if (!user.EmailConfirmed)
        {
            _logger.LogWarning(
                "Login blocked: User {UserId} has unconfirmed email",
                user.Id);
            return Error.Unauthorized("Auth.EmailNotConfirmed", "Confirm email before logging in");
        }

        var accessToken = await _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken(user);

        var isAdmin = await _userManager.IsInRoleAsync(user, Roles.Admin);

        _logger.LogInformation(
            "User {UserId} logged in",
            user.Id);
        var tokens = new TokenResult
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
        var res = new LoginResult
        {
            Tokens = tokens,
            User = user.ToDto(isAdmin)
        };
        return res;
    }

    // usuwa access i refresh token
    // ustawia nowy stamp w userze
    public async Task<ErrorOr<Success>> LogoutFull(int userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return Result.Success;

        await _userManager.UpdateSecurityStampAsync(user);
        _logger.LogInformation(
            "User {UserId} logged out from all sessions",
            user.Id);

        return Result.Success;
    }

    // ustawia emailVerified na true
    // jeśli token i userId sie zgadzaja
    public async Task<ErrorOr<Success>> ConfirmEmail(int userId, string token)
    {
        //var decodedToken = WebUtility.UrlDecode(token);
        var decodedToken = token;
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            _logger.LogWarning(
                "Email confirmation failed: User {UserId} not found",
                userId);
            return Error.NotFound("User.NotFound", "User does not exist");
        }

        if (user.EmailConfirmed) return Result.Success;

        IdentityResult result;
        try
        {
            result = await _userManager.ConfirmEmailAsync(user, decodedToken);
        }
        catch (FormatException)
        {
            _logger.LogWarning(
                "Email confirmation failed: invalid token for user {UserId}",
                userId);
            return Error.Validation("Token.Invalid", "Invalid token");
        }

        if (!result.Succeeded)
        {
            _logger.LogWarning(
                "Email confirmation failed for User {UserId}: {Errors}",
                userId, string.Join(", ", result.Errors.Select(e => e.Code)));
            return result.Errors
                .Select(e => Error.Validation(e.Code, e.Description))
                .ToList();
        }
        _logger.LogInformation(
            "User {UserId} confirmed email",
            userId );

        return Result.Success;
    }

    public async Task<ErrorOr<LoginResult>> RefreshToken(string refreshToken)
    {
        var principal = _tokenService.ValidateRefreshToken(refreshToken);
        if (principal == null)
        {
            _logger.LogWarning(
                "Refresh failed: token failed validation (malformed, expired, bad signature)");
            return Error.Unauthorized("Auth.InvalidRefreshToken", "Refresh token is invalid");
        }

        var userId = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (userId == null)
        {
            _logger.LogWarning(
                "Refresh failed: token is invalid");
            return Error.Unauthorized("Auth.InvalidRefreshToken", "Refresh token is invalid");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            _logger.LogWarning(
                "Refresh failed: User {UserId} not found",
                userId);
            return Error.Unauthorized("Auth.InvalidRefreshToken", "Refresh token is invalid");
        }

        var stampInToken = principal.FindFirstValue("security_stamp");
        if (stampInToken != user.SecurityStamp)
        {
            _logger.LogWarning(
                "Refresh rejected: invalid security_stamp for User {UserId}",
                userId);
            return Error.Unauthorized("Auth.TokenRevoked", "Session expired. Log in again");
        }

        var newAccessToken = await _tokenService.GenerateAccessToken(user);
        var newRefreshToken =
            _tokenService.GenerateRefreshToken(user); // rotacja - nowy refresh przy kazdym odswiezeniu

        var isAdmin = await _userManager.IsInRoleAsync(user, Roles.Admin);
        
        var tokens = new TokenResult { AccessToken = newAccessToken, RefreshToken = newRefreshToken };
        return new LoginResult
        {
            Tokens = tokens,
            User = user.ToDto(isAdmin)
        };
    }


    public async Task<ErrorOr<Success>> ResendConfirmation(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null || user.EmailConfirmed) return Result.Success;

        await SendConfirmationEmail(user);
        _logger.LogInformation("Confirmation email resent to User {UserId}", user.Id);

        return Result.Success;
    }

    public async Task<ErrorOr<Success>> RequestPasswordReset(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || !user.EmailConfirmed) return Result.Success;

        await SendPasswordResetEmail(user);
        _logger.LogInformation("Password reset requested for User {UserId}", user.Id);

        return Result.Success;
    }

    public async Task<ErrorOr<Success>> ResetPassword(int userId, string token, string newPassword)
    {
        //var decodedToken = WebUtility.UrlDecode(token);
        var decodedToken = token;
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            _logger.LogWarning("Password reset failed: User {UserId} not found", userId);
            return Error.Validation("Token.Invalid", "Invalid or expired token");
        }

        IdentityResult result;
        try
        {
            result = await _userManager.ResetPasswordAsync(user, decodedToken, newPassword);
        }
        catch (FormatException)
        {
            _logger.LogWarning(
                "Password reset failed: invalid token for User {UserId}",
                userId);
            return Error.Validation("Token.Invalid", "Invalid or expired token");
        }

        if (!result.Succeeded)
        {
            _logger.LogWarning(
                "Password reset failed for User {UserId}: {Errors}",
                userId, string.Join(", ", result.Errors.Select(e => e.Code)));
            return result.Errors
                .Select(e => Error.Validation(e.Code, e.Description))
                .ToList();
        }

        // unieważnia refresh token
        await _userManager.UpdateSecurityStampAsync(user);
        _logger.LogInformation("Password reset completed for User {UserId}", user.Id);

        return Result.Success;
    }

    public async Task<ErrorOr<Success>> ChangePassword(int userId, string oldPassword, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return Error.NotFound("User.NotFound", "User does not exist");

        var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        if (!result.Succeeded)
        {
            _logger.LogWarning(
                "Password change failed for User {UserId}: {Errors}",
                userId, string.Join(", ", result.Errors.Select(e => e.Code)));
            return result.Errors
                .Select(e => Error.Validation(e.Code, e.Description))
                .ToList();
        }

        await _userManager.UpdateSecurityStampAsync(user);
        await SendPasswordChangedNotificationEmail(user);
        
        _logger.LogInformation("Password changed for User {UserId}", user.Id);

        return Result.Success;
    }

    private async Task SendPasswordChangedNotificationEmail(AppUser user)
    {
        var html = await _templateService.RenderAsync(EmailTemplates.PasswordChanged, new object());
        await _emailSender.SendEmailAsync(user.Email, "Password Changed", html);
    }

    private async Task SendPasswordResetEmail(AppUser user)
    {
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = WebUtility.UrlEncode(token);

        var url = $"{_frontendOptions.Url}/reset-password?userId={user.Id}&token={encodedToken}";
        var html = await _templateService.RenderAsync(EmailTemplates.ResetPassword, new
        {
            ResetUrl = url
        });

        await _emailSender.SendEmailAsync(user.Email,
            "Reset password",
            html
        );
    }

    private async Task SendConfirmationEmail(AppUser user)
    {
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = WebUtility.UrlEncode(token);

        var confirmUrl = $"{_frontendOptions.Url}/confirm-email?userId={user.Id}&token={encodedToken}";

        var html = await _templateService.RenderAsync(EmailTemplates.ConfirmEmail, new
        {
            Username = user.UserName,
            ConfirmUrl = confirmUrl
        });

        // powinien juz być email w userze
        await _emailSender.SendEmailAsync(user.Email,
            "Confirm your email",
            html
        );
    }

    private async Task AssignRoleAsync(AppUser user, string role)
    {
        if (!await _roleManager.RoleExistsAsync(role))
        {
            _logger.LogInformation("Role {Role} created (did not exist)", role);
            await _roleManager.CreateAsync(new IdentityRole<int>(role));
        }

        if (!await _userManager.IsInRoleAsync(user, role))
        {
            _logger.LogInformation("Role {Role} assigned to User {UserId}", role, user.Id);
            await _userManager.AddToRoleAsync(user, role);
        }
    }
}