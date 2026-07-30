using GameNewsHub.Api.Dtos;
using GameNewsHub.Data.Entities;

namespace GameNewsHub.Api.Features.Genres;

public interface IGenreService
{
    public Task<List<GenreResponse>> GetGenreListAsync();
}