using backend.Data;
using backend.Models.Entities;
using backend.Services.Background;
using backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Implementations;

public class GameSyncService : IGameSyncService
{
    private readonly IgdbClient _client;
    private readonly IGenreService _genreService;
    private readonly AppDbContext _context;
    private readonly ILogger<GameSyncService> _logger;

    public GameSyncService(IgdbClient client, IGenreService service, AppDbContext context, ILogger<GameSyncService> logger)
    {
        _client = client;
        _genreService = service;
        _context = context;
        _logger = logger;
    }

    public async Task SyncUpcomingGamesAsync(int limit = 10)
    {
        var rawGames = await _client.GetGamesFromIgdb(limit);

        foreach (var dto in rawGames)
        {
            if (await _context.Games.AnyAsync(g => g.IgdbId == dto.Id)) continue;

            var game = new Game
            {
                IgdbId = dto.Id,
                Title = dto.Name,
                Summary = dto.Summary,
                CoverUrl = dto.Cover?.Url?.Replace("t_thumb", "t_720p").Insert(0, "https:")
            };

            if (dto.Genres != null)
            {
                foreach (var gDto in dto.Genres)
                {
                    var genre = await _genreService.GetOrCreateAsync(gDto.Id, gDto.Name);
                    game.Genres.Add(genre);
                }
            }

            _context.Games.Add(game);
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("Game sync finished.");
    }
}