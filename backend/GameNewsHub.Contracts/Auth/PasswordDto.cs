using System.ComponentModel.DataAnnotations;

namespace GameNewsHub.Contracts.Auth;

public class ResetPasswordDto
{
    [Required] public int UserId { get; set; }
    [Required] public string Token { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = string.Empty;
}

public class ChangePasswordDto
{
    [Required] public string OldPassword { get; set; } = string.Empty;
    [Required] public string NewPassword { get; set; } = string.Empty;
}
