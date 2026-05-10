namespace GameNewsHub.Api.Features.UserInterest.Calculator;

public interface IUserWeightService
{
    public Task UpdateUserWeightsAsync(int userId);
}