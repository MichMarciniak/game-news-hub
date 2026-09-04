using backend.Data;
using GameNewsHub.Api.Dtos;
using GameNewsHub.Api.Mappings;
using GameNewsHub.Contracts;
using GameNewsHub.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameNewsHub.Api.Features.Genres;

public class GenreService 
{
    private readonly AppDbContext _context;

    public GenreService(AppDbContext context)
    {
        _context = context;
    }
    

    public Task<List<GenreDto>> GetGenreListAsync()
    {
        var genres = _context.Genres.Select(g => g.ToDto()).ToList();
        return Task.FromResult(genres);
    }

}