using System.ComponentModel.DataAnnotations;

namespace GameNewsHub.Contracts;

public record RegisterDto
{
    [Required] public string Email { get; set; }
    [Required] [MaxLength(100)] public string Username { get; set; }
    [Required] public string Password { get; set; }
}

public record LoginDto
{
    [Required] public string Email { get; set; }
    [Required] public string Password { get; set; }
}

public class LoginResponse
{
    public string AccessToken { get; set; }
}

public class EmailConfirmDto
{
    [Required] public int UserId { get; set; }
    [Required] public string Token { get; set; }
}

public class EmailDto
{
    [Required] public string Email { get; set; }
}

public class ResetPasswordDto
{
    [Required] public int UserId { get; set; }
    [Required] public string Token { get; set; }
    [Required] public string Password { get; set; }
}

public class ChangePasswordDto
{
    [Required] public string OldPassword { get; set; }
    [Required] public string NewPassword { get; set; }
}