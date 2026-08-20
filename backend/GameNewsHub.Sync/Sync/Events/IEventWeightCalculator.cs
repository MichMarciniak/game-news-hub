using GameNewsHub.Data.Entities;

namespace GameNewsHub.Sync.Sync.Events;

public interface IEventWeightCalculator
{
    public IEnumerable<EventGenreWeight> CalculateScores(int eventId, IEnumerable<Game> games);
}