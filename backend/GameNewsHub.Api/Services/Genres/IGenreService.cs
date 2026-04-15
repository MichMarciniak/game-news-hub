using backend.Models.Entities;

namespace GameNewsHub.Api.Services.Genres;

public interface IGenreService
{
    public Task<Genre> GetOrCreateAsync(int igdbId, string name);
}