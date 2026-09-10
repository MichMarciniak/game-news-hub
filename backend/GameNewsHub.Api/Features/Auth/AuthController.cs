using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using backend.Configuration;
using backend.Extensions;
using ErrorOr;
using GameNewsHub.Api.Features.Shared;
using GameNewsHub.Contracts;
using GameNewsHub.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace GameNewsHub.Api.Features.Auth;

[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly AuthService _service;
    private readonly JwtTokenOptions _tokenOptions;

    public AuthController(AuthService service, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IConfiguration configuration,  IOptions<JwtTokenOptions> tokenOptions)
    {
        _service = service;
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _tokenOptions = tokenOptions.Value;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto request)
    {
        var result = await _service.Register(request);
        return result.Match<IActionResult>(
            ok => Ok(),
            error => BadRequest(error));

    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto request)
    {
        var result = await _service.Login(request);
        return result.Match(
            tokenResult =>
            {
                Response.Cookies.Append("refreshToken", tokenResult.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax, //TODO change to strict
                    Expires = DateTimeOffset.UtcNow.Add(_tokenOptions.RefreshTokenLifetime)
                });
                return Ok(new LoginResponse{AccessToken = tokenResult.AccessToken});
            },
            errors => this.ProblemErr(errors)
        );

    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized();
        }

        var result = await _service.RefreshToken(refreshToken);

        return result.MatchFirst(
            tokenResult =>
            {
                Response.Cookies.Append("refreshToken", tokenResult.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax, //TODO change to strict
                    Expires = DateTimeOffset.UtcNow.Add(_tokenOptions.RefreshTokenLifetime)
                });
                return Ok(new LoginResponse{AccessToken = tokenResult.AccessToken});
            },
            errors => this.ProblemErr(errors)
        );
    }

    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("refreshToken");
        return NoContent();
    }

    [HttpPost("logout-all")]
    [Authorize]
    public async Task<IActionResult> LogoutAll()
    {
        var userId = User.GetUserId();
        await _service.LogoutFull(userId);
        Response.Cookies.Delete("refreshToken");
        return NoContent();
    }

    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(EmailConfirmDto request)
    {
        var result = await _service.ConfirmEmail(request.UserId, request.Token);
        return result.Match<IActionResult>(
            ok => NoContent(),
            error => this.ProblemErr(error));
    }

    [HttpPost("resend-email")]
    public async Task<IActionResult> ResendConfirmation(EmailDto request)
    {
        var result = await _service.ResendConfirmation(request.Email);
        return result.Match<IActionResult>(
            ok => NoContent(),
            error => this.ProblemErr(error));
    }

    [HttpPost("request-password-reset")]
    public async Task<IActionResult> RequestPasswordReset(EmailDto request)
    {
        var result = await _service.RequestPasswordReset(request.Email);
        return result.Match<IActionResult>(
            ok => NoContent(),
            error => this.ProblemErr(error));
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto request)
    {
        var result = await _service.ResetPassword(request.UserId, request.Token, request.Password);
        return result.Match<IActionResult>(
            ok => NoContent(),
            error => this.ProblemErr(error));
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto request)
    {
        var userId = User.GetUserId();
        var result = await _service.ChangePassword(userId, request.OldPassword, request.NewPassword);
        return result.Match<IActionResult>(
            ok => NoContent(),
            error => this.ProblemErr(error));
    }
}