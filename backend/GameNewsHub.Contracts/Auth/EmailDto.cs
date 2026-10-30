using System.ComponentModel.DataAnnotations;

namespace GameNewsHub.Contracts.Auth;

public class EmailConfirmDto
{
    [Required] public int UserId { get; set; }
    [Required] public string Token { get; set; } = string.Empty;
}

public class EmailDto
{
    [Required] public string Email { get; set; } = string.Empty;
}
