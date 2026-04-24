using backend.Models.Entities;

namespace GameNewsHub.Api.Logic.Events;

public interface IEventWeightCalculator
{
    public IEnumerable<EventGenreWeight> CalculateScores(int eventId, IEnumerable<Game> games);
}