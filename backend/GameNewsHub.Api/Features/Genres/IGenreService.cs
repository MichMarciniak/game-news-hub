using GameNewsHub.Api.Entities;
using GameNewsHub.Contracts;

namespace GameNewsHub.Api.Features.Genres;

public interface IGenreService
{
    public Task<List<GenreDto>> GetGenreListAsync();
}