namespace GameNewsHub.Sync.Options;

public class ApiOptions
{
    public const string SectionName = "Api";
    public string BaseUrl { get; set; }
    public string TokenUrl { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
}