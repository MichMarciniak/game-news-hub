using backend.Models.DTOs;

namespace GameNewsHub.Api.External;

public interface IIgdbClient
{
    public Task<List<GameResponse>> GetGamesFromIgdb(int limit = 5);
}