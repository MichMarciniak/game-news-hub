using GameNewsHub.Api.Entities;

namespace GameNewsHub.Api.Sync.Events;

public interface IEventWeightCalculator
{
    public IEnumerable<EventGenreWeight> CalculateScores(int eventId, IEnumerable<Game> games);
}