using System.Globalization;
using GameNewsHub.Contracts;
using GameNewsHub.Data;
using GameNewsHub.External.Igdb.Dtos;
using GameNewsHub.External.Steam.Client;
using GameNewsHub.Sync.Core.Games;
using Microsoft.EntityFrameworkCore;

namespace GameNewsHub.Api.Features.Steam;

public class SteamService
{
    private readonly AppDbContext _context;
    private readonly SteamClient _client;
    private readonly ILogger<SteamService> _logger;
    private readonly IGameSyncService _gameSyncService;

    public SteamService(
        AppDbContext context,
        SteamClient steamClient,
        ILogger<SteamService> logger,
        IGameSyncService gameSyncService
    )
    {
        _context = context;
        _client = steamClient;
        _logger = logger;
        _gameSyncService = gameSyncService;
    }

    public async Task<SteamGameWrapperDto> GetSteamGamesFromProfile(string url)
    {
        var steamUserId = GetUserIdFromUrl(url);

        var ownedGamesResponse = await _client.GetSteamOwnedGames(steamUserId);
        if (ownedGamesResponse?.Games == null || !ownedGamesResponse.Games.Any())
        {
            return new SteamGameWrapperDto();
        }

        var steamAppIds = ownedGamesResponse.Games.Select(g => g.AppId).ToList();

        var dbGames = await _context.Games
            .Where(g => g.SteamAppId != null && steamAppIds.Contains(g.SteamAppId.Value))
            .ToListAsync();

        var missingSteamIds = steamAppIds.Where(i => 
            !dbGames.Select(g => g.SteamAppId!.Value).Contains(i))
            .Select(x => x.ToString());
        
        // fetch games
        var missingGames = await _gameSyncService.ImportByExternalIdsAsync(missingSteamIds, ExternalGameSource.Steam);

        var allGames = missingGames.Concat(dbGames);
        // user games bo ten import zwraca też pobrane parent games
        var userGames = allGames
            .Where(g => g.SteamAppId.HasValue && steamAppIds.Contains(g.SteamAppId.Value))
            .DistinctBy(g => g.SteamAppId)
            .ToList();
        
        var playtimeDict = ownedGamesResponse.Games.ToDictionary(g => g.AppId, g => g.PlaytimeForeverMinutes);

        var lastPlayedDate = ownedGamesResponse.Games.ToDictionary(g => g.AppId,
            g => DateTimeOffset.FromUnixTimeSeconds(g.RtimeLastPlayedUnixSeconds));

        var result = userGames 
            .Select(game => new SteamGameDto
            {
                Id = game.Id,
                SteamAppId = game.SteamAppId!.Value,
                Title = game.Title,
                CoverUrl = game.CoverUrl,
                PlaytimeForeverMinutes = playtimeDict[game.SteamAppId!.Value],
                LastPlayed = lastPlayedDate[game.SteamAppId.Value]
            }).ToList();

        return new SteamGameWrapperDto
        {
            Requested = steamAppIds.Count,
            Found = result.Count,
            Games = result
        };
    }

    private ulong GetUserIdFromUrl(string url)
    {
        if (ulong.TryParse(url, out var directId))
        {
            return directId;
        }

        if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            var segments = uri.Segments
                .Select(s => s.Trim('/'))
                .Where(s => !string.IsNullOrEmpty(s))
                .ToList();

            int profilesIndex = segments.IndexOf("profiles");
            if (profilesIndex != -1 && segments.Count > profilesIndex + 1)
            {
                if (ulong.TryParse(segments[profilesIndex + 1], out var steamId))
                {
                    return steamId;
                }
            }
        }

        throw new ArgumentException("Invalid steam url or id");
    }
    
    
}
