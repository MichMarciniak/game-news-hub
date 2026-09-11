using GameNewsHub.Data.Entities;
using GameNewsHub.Sync.Dtos;

namespace GameNewsHub.Sync.Sync.Genres;

public interface IGenreSyncService
{
    public Task<Genre> GetOrCreateAsync(int igdbId, string name);
    public Task<ICollection<Genre>> GetOrCreateBatchAsync(IEnumerable<IgdbGenreResponse> genreDtos);
}