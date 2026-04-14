using System.Dynamic;
using backend.Data;
using backend.Models.Entities;
using backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GameNewsHub.Api.Services;

public class GenreService : IGenreService
{
    private readonly AppDbContext _context;

    public GenreService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<Genre> GetOrCreateAsync(int igdbId, string name)
    {
        if (igdbId < 0)
            throw new ArgumentException("Igdb Id cannot be < 0", nameof(name));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Genre name cannot be empty", nameof(name));
        
        var genre = await _context.Genres.FirstOrDefaultAsync(g => g.IgdbId == igdbId);

        if (genre != null) return genre;
        
        genre = new Genre {IgdbId = igdbId, Name = name};
        _context.Genres.Add(genre);

        return genre;
    }
}