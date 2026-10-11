using System.ComponentModel.DataAnnotations;
using GameNewsHub.Hosting.Configuration;

namespace GameNewsHub.Api.Options;

public class SmtpOptions : IConfigSection
{
    public static string SectionName => "Smtp";
    [Required] public string Host { get; set; }
    [Required] public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    [Required] public bool EnableSsl { get; set; }
    [Required] public string FromEmail { get; set; }
    [Required] public string FromName { get; set; }
}