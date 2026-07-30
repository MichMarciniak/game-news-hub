using ErrorOr;

namespace GameNewsHub.Api.Features.Games;

public interface IGameService
{
    public Task<List<GameListResponse>> GetGamesListAsync();
    public Task<List<GameListResponse>> SearchGamesAsync(string query);
    public Task<ErrorOr<GameDetailsResponse>>  GetGameDetailsAsync(int gameId);
}