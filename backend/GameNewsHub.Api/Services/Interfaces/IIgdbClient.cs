using backend.Models.DTOs;

namespace GameNewsHub.Api.Services.Interfaces;

public interface IIgdbClient
{
    public Task<List<GameResponse>> GetGamesFromIgdb(int limit = 5);
}