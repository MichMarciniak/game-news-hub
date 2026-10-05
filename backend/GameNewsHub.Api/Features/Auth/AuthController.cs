using GameNewsHub.Api.Extensions;
using GameNewsHub.Api.Features.Shared;
using GameNewsHub.Api.Options;
using GameNewsHub.Contracts;
using GameNewsHub.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GameNewsHub.Api.Features.Auth;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _service;

    public AuthController(AuthService service)
    {
        _service = service;
    }

    [HttpGet("test")]
    [Authorize]
    public async Task<IActionResult> Test()
    {
        var userId = User.GetUserId();
        if (userId == null) return NotFound();
        return Ok();
    }

    [HttpPost("register")]
    public async Task<ActionResult<EmailDto>> Register(RegisterDto request)
    {
        var result = await _service.Register(request);
        return result.Match(
            details => Ok(details),
            error => this.ProblemErr(error));
    }

    [HttpPost("login")]
    public async Task<ActionResult<ProfileDto>> Login(LoginDto request)
    {
        var result = await _service.Login(request);
        return result.Match(
            value => Ok(value),
            errors => this.ProblemErr(errors)
        );
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _service.Logout();
        return NoContent();
    }

    [HttpPost("logout-all")]
    [Authorize]
    public async Task<IActionResult> LogoutAll()
    {
        var userId = User.GetUserId();
        await _service.LogoutFull(userId);
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

    [HttpPost("forgot-password")]
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