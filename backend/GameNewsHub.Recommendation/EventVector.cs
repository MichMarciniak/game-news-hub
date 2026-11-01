namespace GameNewsHub.Recommendation;

public class EventVector
{
    public int EventId { get; set; }
    public int SeriesId { get; set; }
    public int FollowerCount { get; set; }

    public DateTimeOffset StartTime { get; set; }
    public HashSet<int> DirectGameIds { get; set; } = [];
    public HashSet<int> ParentGameIds { get; set; } = [];

    public Dictionary<int, float> GenreWeights { get; set; } = [];
    public Dictionary<int, float> PlatformGroupWeights { get; set; } = [];
}