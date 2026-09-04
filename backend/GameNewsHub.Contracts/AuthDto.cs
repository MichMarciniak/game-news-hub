using System.ComponentModel.DataAnnotations;

namespace GameNewsHub.Contracts;

public record RegisterDto
{
    [Required] [MaxLength(100)] public string Username { get; set; }
    [Required] public string Password { get; set; }
}

public record LoginDto 
{
    [Required] [MaxLength(100)] public string Username { get; set; }
    [Required] public string Password { get; set; }
}

