using GameNewsHub.Api.Entities;
using GameNewsHub.Contracts;

namespace GameNewsHub.Api.Features.Genres;

public interface IGenreService
{
    // TODO: move creates to sync
    public Task<Genre> GetOrCreateAsync(int igdbId, string name);
    public Task<ICollection<Genre>> GetOrCreateBatchAsync(IEnumerable<GenreDto> genreDtos);

    public Task<List<GenreDto>> GetGenreListAsync();
}