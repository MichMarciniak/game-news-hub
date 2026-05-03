using backend.Models.DTOs;
using backend.Models.Entities;

namespace GameNewsHub.Api.Services.Genres;

public interface IGenreService
{
    public Task<Genre> GetOrCreateAsync(int igdbId, string name);
    public Task<ICollection<Genre>> GetOrCreateBatchAsync(IEnumerable<GenreDto> genreDtos);
}