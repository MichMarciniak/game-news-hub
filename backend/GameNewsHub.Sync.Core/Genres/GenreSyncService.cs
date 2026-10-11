using GameNewsHub.Data;
using GameNewsHub.Data.Entities;
using GameNewsHub.External.Igdb.Dtos;
using Microsoft.EntityFrameworkCore;

namespace GameNewsHub.Sync.Core.Genres;

public class GenreSyncService : IGenreSyncService
{
    private readonly AppDbContext _context;

    public GenreSyncService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Genre>> GetOrCreateBatchAsync(List<IgdbGenreResponse>? genreDtos)
    {
        if (genreDtos == null || !genreDtos.Any()) return new List<Data.Entities.Genre>();

        var existing= await GetExistingGenres(genreDtos.ToList());

        var existingIds = existing.Select(g => g.IgdbId);
        
        var missingDtos = genreDtos
            .Where(d => !existingIds.Contains(d.IgdbId))
            .ToList();

        var newGenres = missingDtos
            .Select(dto => new Genre
            {
                IgdbId = dto.IgdbId,
                Name = dto.Name
            }).ToList();

        if (newGenres.Any())
        {
            _context.Genres.AddRange(newGenres);
            await _context.SaveChangesAsync();
            existing.AddRange(newGenres);
        }
        return existing;
    }

    private async Task<List<Genre>> GetExistingGenres(List<IgdbGenreResponse> genreDtos)
    {
        var ids = genreDtos.Select(g => g.IgdbId);
        var existing = await _context.Genres
            .Where(g => ids.Contains(g.IgdbId))
            .ToListAsync();

        return existing;
    }

}