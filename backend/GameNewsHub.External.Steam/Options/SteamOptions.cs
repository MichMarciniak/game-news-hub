using System.ComponentModel.DataAnnotations;
using GameNewsHub.Hosting.Configuration;

namespace GameNewsHub.External.Steam.Options;

public class SteamOptions : IConfigSection
{
    public static string SectionName => "Steam";
    [Required] public string Key { get; set; }
}