using backend.Data;

namespace GameNewsHub.Api.Features.Games;

public class GameService : IGameService
{
    private readonly AppDbContext _context;

    public GameService(AppDbContext context)
    {
        _context = context;
    }


    public Task<List<int>> GetGamesListAsync()
    {
        throw new NotImplementedException();
    }

    public Task<List<int>> SearchGamesAsync(string query)
    {
        throw new NotImplementedException();
    }

    public Task<object> GetGameDetailsAsync(int gameId)
    {
        throw new NotImplementedException();
    }
}