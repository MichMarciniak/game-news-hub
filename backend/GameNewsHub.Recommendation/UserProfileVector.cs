namespace GameNewsHub.Recommendation;

public class UserProfileVector
{
    public int UserId { get; set; }

    public Dictionary<int, float> GameWeights { get; set; } = [];
    public Dictionary<int, float> GenreWeights { get; set; } = [];
    public Dictionary<int, float> PlatformGroupWeights { get; set; } = [];
    public Dictionary<int, float> SeriesWeights { get; set; } = [];

    // wyliczony dynamicznie współczynnik personalizcji (0.0 - 1.0)
    public float Alpha { get; set; }

}