using System.ComponentModel.DataAnnotations;
using GameNewsHub.Hosting.Configuration;

namespace GameNewsHub.Sync.Options;

public class SyncRangeOptions : IConfigSection
{
    public static string SectionName => "SyncRange";
    [Required] public int Days { get; set; }
}