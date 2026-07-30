using GameNewsHub.Api.Dtos;

namespace GameNewsHub.Api.Features.Recommendations;

public interface IRecommendationService
{
    public Task<List<EventRecommendationResponse>> GetRecommendedEventList(int userId);
}