using System.ComponentModel.DataAnnotations;

namespace GameNewsHub.Contracts;

public record RegisterRequest
{
    [Required] [MaxLength(100)] public string Username { get; set; }

    [Required] public string Password { get; set; }
};

public record LoginRequest
{
    [Required] [MaxLength(100)] public string Username { get; set; }

    [Required] public string Password { get; set; }
};