using System.ComponentModel.DataAnnotations;
using GameNewsHub.Hosting.Configuration;

namespace GameNewsHub.External.Steam.Options;

public class SteamOptions : IConfigSection
{
    public static string SectionName => "Steam";
    [Required] public string AuthUrl { get; set; }
    [Required] public string ApiUrl { get; set; }
    [Required] public string Key { get; set; }

    // identyfikator aplikacji - zwykle główna domena
    [Required] public string Realm { get; set; }
    [Required] public string ReturnTo { get; set; }
}