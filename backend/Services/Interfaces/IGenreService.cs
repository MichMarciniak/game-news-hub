using backend.Models.DTOs;
using backend.Models.Entities;

namespace backend.Services.Interfaces;

public interface IGenreService
{
    public Task<Genre> GetOrCreateAsync(int igdbId, string name);
}