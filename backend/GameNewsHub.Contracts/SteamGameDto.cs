namespace GameNewsHub.Contracts;

public class SteamGameWrapperDto 
{
    public int Requested { get; set; }
    public int Found { get; set; }
    public List<SteamGameDto> Games { get; set; }
}

public class SteamGameDto
{
    public int Id { get; set; }
    public ulong SteamAppId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? CoverUrl { get; set; }
    public long PlaytimeForeverMinutes { get; set; }
    public DateTimeOffset LastPlayed { get; set; }
}

