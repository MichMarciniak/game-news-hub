using backend.Models.Entities;

namespace GameNewsHub.Api.Services.Events;

public interface IEventWeightCalculator
{
    public IEnumerable<EventGenreWeight> CalculateScores(int eventId, IEnumerable<Game> games);
}