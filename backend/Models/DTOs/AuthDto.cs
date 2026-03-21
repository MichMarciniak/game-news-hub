using System.ComponentModel.DataAnnotations;

namespace backend.Models.DTOs;

public static class AuthDto
{
    public record Register(
        [Required] [MaxLength(100)] string Username,
        [Required] string Password
    );

    public record Login(
        [Required] [MaxLength(100)] string Username,
        [Required] string Password
    );
}