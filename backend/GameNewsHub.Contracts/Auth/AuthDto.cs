using System.ComponentModel.DataAnnotations;
using GameNewsHub.Contracts.User;

namespace GameNewsHub.Contracts.Auth;

public record RegisterDto
{
    [Required] [EmailAddress] public string Email { get; set; } = string.Empty;
    [Required] [MaxLength(100)] public string Username { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = string.Empty;
}

public record LoginDto
{
    [Required] public string Email { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = string.Empty;
}