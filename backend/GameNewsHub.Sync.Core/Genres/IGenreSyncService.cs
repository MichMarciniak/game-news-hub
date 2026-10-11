using GameNewsHub.Data.Entities;
using GameNewsHub.External.Igdb.Dtos;

namespace GameNewsHub.Sync.Core.Genres;

public interface IGenreSyncService
{
    public Task<List<Genre>> GetOrCreateBatchAsync(List<IgdbGenreResponse> genreDtos);
}