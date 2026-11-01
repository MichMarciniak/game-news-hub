using GameNewsHub.Data.Entities;

namespace GameNewsHub.Recommendation;

/// <summary>
/// Buduje wektory dla wydarzenia, uwzględniając relację ParentGame (np. dodatek)
/// </summary>
public class EventVectorRepresenter
{
    public Task<EventVector> BuildEventVectorAsync(Event evt, CancellationToken ct)
    {
        var vector = new EventVector
        {
            EventId = evt.Id,
            SeriesId = evt.EventSeriesId,
            FollowerCount = evt.FollowerCount,
            StartTime = evt.StartTime
        };

        foreach (var game in evt.Games)
        {
            vector.DirectGameIds.Add(game.Id);

            if (game.ParentGameId.HasValue)
            {
                vector.ParentGameIds.Add(game.ParentGameId.Value);
            }

            foreach (var genre in game.Genres)
            {
                AddWeight(vector.GenreWeights, genre.Id, 1.0f);
            }

            var platformGroupIds = game.Platforms
                .Select(p => p.PlatformGroupId)
                .Distinct();

            foreach (var pgi in platformGroupIds)
            {
                AddWeight(vector.PlatformGroupWeights, pgi, 1.0f);
            }

        }

        return Task.FromResult(vector);
    }
    
    public async Task<List<EventVector>> BuildEventVectorListAsync(IEnumerable<Event> events, CancellationToken ct)
    {

        var vectors = new List<EventVector>();
        foreach (var evt in events)
        {
            vectors.Add(await BuildEventVectorAsync(evt, ct));
        }

        return vectors;
    }

    private static void AddWeight(Dictionary<int, float> dict, int key, float weight)
    {
        if (dict.TryGetValue(key, out var current))
        {
            dict[key] = current + weight;
        }
        else
        {
            dict[key] = weight;
        }
    }
}