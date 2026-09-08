using System.Text.RegularExpressions;

namespace GameNewsHub.Sync.Sync.Events;

public static class EventNameNormalizer
{

    public static string Normalize(string name)
    {
        var cleanName = name;
        cleanName = Regex.Replace(cleanName,
            "\\d{1,4}[./-]\\d{1,2}([./-]\\d{1,4})?",
            " ");
        cleanName = Regex.Replace(cleanName,
            "\\d{1,2}th|1st|2nd|3rd",
            " ");
        cleanName = Regex.Replace(cleanName,
            "\\b(January|February|March|April|May|June|July|August|September|October|November|December)\\b", 
            " ", RegexOptions.IgnoreCase);
        cleanName = Regex.Replace(cleanName,
            "\\b(Winter|Spring|Summer|Fall|Autumn|Edition)\\b",
            " ", RegexOptions.IgnoreCase);
        cleanName = Regex.Replace(cleanName,
            "vol\\.?\\s*\\d+",
            " ", RegexOptions.IgnoreCase);
        cleanName = Regex.Replace(cleanName,
            "[-:_,|+./–—]", //jakies emdashe byly whyyy
            " ");
        cleanName = Regex.Replace(cleanName,
            "(^|'|#)\\d*[0-9]($)", 
            " ");
        cleanName = Regex.Replace(cleanName,
            "\\b\\d+\\b",
            " ");
        cleanName = Regex.Replace(cleanName,
            "\\s{2,}",
            " ");
        cleanName = Regex.Replace(cleanName,
            "(^\\s+)|(\\s+$)",
            "");
        
        return cleanName;
    }
}