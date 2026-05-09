using ErrorOr;

namespace GameNewsHub.Api.Features.UserInterest;

public interface IUserInterestService
{
    
    public Task<ErrorOr<Success>> ToggleFollowGameAsync(int gameId, int userId);
    
    public Task<ErrorOr<Success>> ToggleFollowGenreAsync(int genreId, int userId);
    
    public Task<ErrorOr<List<int>>> GetFollowedGamesAsync(int userId);
    public Task<ErrorOr<List<int>>> GetFollowedGenresAsync(int userId);
    
}