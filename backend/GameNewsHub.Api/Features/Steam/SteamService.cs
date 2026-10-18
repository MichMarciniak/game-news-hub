using System.Globalization;
using GameNewsHub.Contracts;
using GameNewsHub.Data;
using GameNewsHub.External.Steam.Client;
using Microsoft.EntityFrameworkCore;

namespace GameNewsHub.Api.Features.Steam;

public class SteamService
{
    private readonly AppDbContext _context;
    private readonly SteamClient _client;
    private readonly ILogger<SteamService> _logger;

    public SteamService(
        AppDbContext context,
        SteamClient steamClient,
        ILogger<SteamService> logger
    )
    {
        _context = context;
        _client = steamClient;
        _logger = logger;
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

        var playtimeDict = ownedGamesResponse.Games.ToDictionary(g => g.AppId, g => g.PlaytimeForeverMinutes);

        var lastPlayedDate = ownedGamesResponse.Games.ToDictionary(g => g.AppId,
            g => DateTimeOffset.FromUnixTimeSeconds(g.RtimeLastPlayedUnixSeconds));

        var result = dbGames
            .Where(g => g.SteamAppId.HasValue)
            .Select(game => new SteamGameDto
            {
                Id = game.Id,
                SteamAppId = game.SteamAppId ?? 0,
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
