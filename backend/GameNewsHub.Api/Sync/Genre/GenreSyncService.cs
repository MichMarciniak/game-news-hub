using backend.Data;
using GameNewsHub.Api.Entities;
using GameNewsHub.Api.Features.Genres;
using GameNewsHub.Contracts;
using Microsoft.EntityFrameworkCore;

namespace GameNewsHub.Api.Sync;

public class GenreSyncService : IGenreSyncService
{
    private readonly AppDbContext _context;

    public GenreSyncService(AppDbContext context)
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

        genre = await AddGenre(igdbId, name);

        return genre;
    }

    public async Task<ICollection<Genre>> GetOrCreateBatchAsync(IEnumerable<GenreDto> genreDtos)
    {
        if (genreDtos == null || !genreDtos.Any())
        {
            return new List<Genre>();
        }

        var incomingIds = genreDtos.Select(g => g.Id).ToList();

        var existing = await _context.Genres
            .Where(g => incomingIds.Contains(g.IgdbId))
            .ToListAsync();
        
        var missing = genreDtos.Except(existing.Select(g => g.ToDto()));

        if (missing.Any())
        {
            var newGenres = missing.Select(g => new Genre
            {
                IgdbId = g.Id,
                Name = g.Name
            }).ToList();
            
            _context.Genres.AddRange(newGenres);
            await _context.SaveChangesAsync();
            
            existing.AddRange(newGenres);
        }

        return existing;
    }
    private async Task<Genre> AddGenre(int igdbId, string name)
    {
        var genre = new Genre {IgdbId = igdbId, Name = name};
        _context.Genres.Add(genre);
        await _context.SaveChangesAsync();

        return genre;
    }
}