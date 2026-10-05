using System.ComponentModel.DataAnnotations;

namespace GameNewsHub.Sync.Options;

public class SeqOptions : IConfigSection 
{
    public static string SectionName => "Seq";
    [Required] public string Url { get; init; } = "";
}