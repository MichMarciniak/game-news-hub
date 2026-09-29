using System.ComponentModel.DataAnnotations;

namespace GameNewsHub.Api.Options;

public class SeqOptions : IConfigSection 
{
    public static string SectionName => "Seq";
    [Required] public string Url { get; init; } = "";
}