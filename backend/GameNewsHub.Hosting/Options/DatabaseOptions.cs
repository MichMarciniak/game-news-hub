using System.ComponentModel.DataAnnotations;
using GameNewsHub.Hosting.Configuration;

namespace GameNewsHub.Hosting.Options;

public class DatabaseOptions : IConfigSection
{
    public static string SectionName => "Database";
    [Required] public string DefaultConnection { get; set; } = string.Empty;
}