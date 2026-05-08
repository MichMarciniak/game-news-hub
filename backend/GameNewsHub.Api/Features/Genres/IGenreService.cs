using GameNewsHub.Api.Entities;
using GameNewsHub.Contracts;

namespace GameNewsHub.Api.Features.Genres;

public interface IGenreService
{
    public Task<Genre> GetOrCreateAsync(int igdbId, string name);
    public Task<ICollection<Genre>> GetOrCreateBatchAsync(IEnumerable<GenreDto> genreDtos);
}