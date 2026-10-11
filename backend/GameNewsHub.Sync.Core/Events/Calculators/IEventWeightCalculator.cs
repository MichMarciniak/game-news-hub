using GameNewsHub.Data.Entities;

namespace GameNewsHub.Sync.Core.Events.Calculators;

public interface IEventWeightCalculator
{
    public IEnumerable<EventGenreWeight> CalculateScores(int eventId, IEnumerable<Game> games);
}