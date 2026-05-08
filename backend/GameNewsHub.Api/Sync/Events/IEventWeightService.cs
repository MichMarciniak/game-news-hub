using GameNewsHub.Api.Entities;

namespace GameNewsHub.Api.Sync.Events;

public interface IEventWeightService
{
    void CalculateWeights(Event dbEvent);
}