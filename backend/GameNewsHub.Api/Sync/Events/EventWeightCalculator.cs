using GameNewsHub.Api.Entities;

namespace GameNewsHub.Api.Sync.Events;

public class EventWeightCalculator : IEventWeightCalculator
{
    public IEnumerable<EventGenreWeight> CalculateScores(int eventId, IEnumerable<Game> games)
    {
        var allGenresInEvent = games
            .SelectMany(g => g.Genres)
            .ToList();

        if (!allGenresInEvent.Any()) return Enumerable.Empty<EventGenreWeight>();

        double totalTags = allGenresInEvent.Count;

        return allGenresInEvent
            .GroupBy(g => g.IgdbId)
            .Select(group => new EventGenreWeight
            {
                EventId = eventId,
                GenreId = group.First().Id,
                Weight = group.Count() / totalTags
            });
    }
}