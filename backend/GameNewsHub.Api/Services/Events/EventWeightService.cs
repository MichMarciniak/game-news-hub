using backend.Models.Entities;

namespace GameNewsHub.Api.Services.Events;

public class EventWeightService : IEventWeightService
{
    public void CalculateWeights(Event dbEvent)
    {
        if (dbEvent.Games == null || !dbEvent.Games.Any()) return;
        
        dbEvent.GenreWeights.Clear();

        var allGenres = dbEvent.Games
            .SelectMany(g => g.Genres)
            .ToList();

        if (!allGenres.Any()) return;
            
    }
}