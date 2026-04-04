using System.ComponentModel.DataAnnotations;

namespace backend.Models.DTOs;

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