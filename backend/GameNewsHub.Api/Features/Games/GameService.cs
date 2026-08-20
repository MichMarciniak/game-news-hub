using backend.Data;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace GameNewsHub.Api.Features.Games;

public class GameService 
{
    private readonly AppDbContext _context;

    public GameService(AppDbContext context)
    {
        _context = context;
    }


    public async Task<List<GameListResponse>> GetGamesListAsync()
    {
        var games = await _context.Games.Select(g => g.ToListResponse()).ToListAsync();
        return games;
    }

    public Task<List<GameListResponse>> SearchGamesAsync(string query)
    {
        throw new NotImplementedException();
    }

    public async Task<ErrorOr<GameDetailsResponse>> GetGameDetailsAsync(int gameId)
    {
        var game = await _context.Games
            .FirstOrDefaultAsync(g => g.Id == gameId);

        if (game == null)
        {
            return Error.NotFound("Game.NotFound", $"Game with id {gameId} not found.");
        }
        
        return game.ToDetailsResponse();
    }
}