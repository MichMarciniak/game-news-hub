using GameNewsHub.Api.Extensions;
using GameNewsHub.Contracts;
using GameNewsHub.Contracts.Auth;
using GameNewsHub.Contracts.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

    [HttpPost("register")]
    public async Task<ActionResult<EmailDto>> PostRegister(RegisterDto request)
    {
        var result = await _service.Register(request);
        return result.ToActionResult(this);
    }

    [HttpPost("login")]
    public async Task<ActionResult<ProfileDto>> PostLogin(LoginDto request)
    {
        var result = await _service.Login(request);
        return result.ToActionResult(this);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> PostLogout()
    {
        await _service.Logout();
        return NoContent();
    }

    [HttpPost("logout-all")]
    [Authorize]
    public async Task<IActionResult> PostLogoutAll()
    {
        var userId = User.GetId();
        await _service.LogoutFull(userId);
        return NoContent();
    }

    [HttpPost("confirm-email")]
    public async Task<IActionResult> PostConfirmEmail(EmailConfirmDto request)
    {
        var result = await _service.ConfirmEmail(request.UserId, request.Token);
        return result.ToNoContent(this);
    }

    [HttpPost("resend-email")]
    public async Task<IActionResult> PostResendConfirmation(EmailDto request)
    {
        var result = await _service.ResendConfirmation(request.Email);
        return result.ToNoContent(this);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> PostRequestPasswordReset(EmailDto request)
    {
        var result = await _service.RequestPasswordReset(request.Email);
        return result.ToNoContent(this);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> PostResetPassword(ResetPasswordDto request)
    {
        var result = await _service.ResetPassword(request.UserId, request.Token, request.Password);
        return result.ToNoContent(this);
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> PostChangePassword(ChangePasswordDto request)
    {
        var userId = User.GetId();
        var result = await _service.ChangePassword(userId, request.OldPassword, request.NewPassword);
        return result.ToNoContent(this);
    }
}