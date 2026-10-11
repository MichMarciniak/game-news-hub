using System.ComponentModel.DataAnnotations;
using GameNewsHub.Hosting.Configuration;

namespace GameNewsHub.Hosting.Options;

public class SeqOptions : IConfigSection 
{
    public static string SectionName => "Seq";
    [Required] public string Url { get; init; } = "";
}