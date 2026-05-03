using backend.Models.Entities;

namespace GameNewsHub.Api.Services.Events;

public interface IEventWeightService
{
    void CalculateWeights(Event dbEvent);
}