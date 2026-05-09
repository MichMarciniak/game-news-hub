namespace GameNewsHub.Api.Features.Games;

public interface IGameService
{
    public Task<List<int>> GetGamesListAsync();
    public Task<List<int>> SearchGamesAsync(string query);
    public Task<object> GetGameDetailsAsync(int gameId);
}