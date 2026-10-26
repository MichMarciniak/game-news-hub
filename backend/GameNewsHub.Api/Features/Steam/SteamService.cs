using System.Globalization;
using ErrorOr;
using GameNewsHub.Contracts;
using GameNewsHub.Data;
using GameNewsHub.Data.Entities;
using GameNewsHub.External.Igdb.Dtos;
using GameNewsHub.External.Steam.Client;
using GameNewsHub.Sync.Core.Games;
using Microsoft.EntityFrameworkCore;

namespace GameNewsHub.Api.Features.Steam;

public class SteamService
{
    private readonly AppDbContext _context;
    private readonly SteamApiClient _apiClient;
    private readonly ILogger<SteamService> _logger;
    private readonly IGameSyncService _gameSyncService;

    public SteamService(
        AppDbContext context,
        SteamApiClient steamApiClient,
        ILogger<SteamService> logger,
        IGameSyncService gameSyncService
    )
    {
        _context = context;
        _apiClient = steamApiClient;
        _logger = logger;
        _gameSyncService = gameSyncService;
    }

    public async Task<ErrorOr<SteamGameWrapperDto>> ImportOwnedGames(int userId)
    {
        var userResult = await  ValidateUserSteamIdAsync(userId);
        if (userResult.IsError) return userResult.Errors;

        var steamId = userResult.Value;
        var ownedGamesResponse = await _apiClient.GetOwnedGamesAsync(steamId);
        var ownedGameList = ownedGamesResponse.Response;

        if (!ownedGameList.Games.Any())
        {
            return new SteamGameWrapperDto();
        }

        var gameList = ownedGameList.Games;
        var steamAppIds = gameList.Select(g => g.AppId).ToList();

        var allGames = await GetOrSyncGamesBySteamIdsAsync(steamAppIds);

        var ownedGamesLookup = gameList
            .GroupBy(g => g.AppId)
            .ToDictionary(g => g.Key, g => g.First());

        var games = allGames 
            .Where(g => ownedGamesLookup.ContainsKey(g.SteamAppId!.Value))
            .Select(g =>
                {
                    var steamData = ownedGamesLookup[g.SteamAppId!.Value];
                    return new SteamGameDto
                    {
                        Id = g.Id,
                        SteamAppId = g.SteamAppId!.Value,
                        Title = g.Title,
                        CoverUrl = g.CoverUrl,
                        PlaytimeForeverMinutes = steamData.PlaytimeForeverMinutes,
                        LastPlayed =
                            DateTimeOffset.FromUnixTimeSeconds(steamData.RtimeLastPlayedUnixSeconds)
                    };
                })
            .ToList();
        
        _logger.LogInformation("User {UserId} imported owned games", userId);

        return new SteamGameWrapperDto
        {
            Requested = steamAppIds.Count,
            Found = games.Count,
            Games = games
        };

    }

    public async Task<ErrorOr<WishlistDto>> ImportWishlist(int userId)
    {
        var userResult = await  ValidateUserSteamIdAsync(userId);
        if (userResult.IsError) return userResult.Errors;

        var steamId = userResult.Value;
        var wishlist = await _apiClient.GetWishlistAsync(steamId);

        if (wishlist == null || !wishlist.Response.Items.Any())
        {
            return new WishlistDto
            {
                ImportedCount = 0,
                AllCount = 0
            };
        }

        var gameList = wishlist.Response.Items;

        var steamAppIds = gameList.Select(g => g.AppId).Distinct().ToList();

        var allGames = await GetOrSyncGamesBySteamIdsAsync(steamAppIds);
        
        var user = await _context.Users
            .Include(u => u.WishlistedGames)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            return Error.NotFound("User not found");

        var newGames = allGames.ExceptBy(user.WishlistedGames.Select(g => g.Id), g => g.Id);

        foreach (var game in newGames)
        {
            user.WishlistedGames.Add(game);
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("User {UserId} imported wishlist", userId);

        return new WishlistDto
        {
            ImportedCount = newGames.Count(),
            AllCount = steamAppIds.Count
        };
    }
    
    private async Task<ErrorOr<string>> ValidateUserSteamIdAsync(int userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return Error.NotFound("User not found");
        }

        var steamId = user.SteamId;
        if (steamId == null)
        {
            return Error.Failure("Account is not connected to steam");
        }

        return steamId;
    }
    private async Task<List<Game>> GetOrSyncGamesBySteamIdsAsync(List<ulong> steamAppIds)
    {
        if (!steamAppIds.Any())
            return new List<Game>();
        
        var dbGames = await _context.Games
            .Where(g => g.SteamAppId.HasValue && steamAppIds.Contains(g.SteamAppId.Value))
            .ToListAsync();

        var dbSteamAppIds = dbGames
            .Select(g => g.SteamAppId!.Value)
            .ToHashSet();

        var missingSteamIds = steamAppIds
            .Where(id => !dbSteamAppIds.Contains(id))
            .Select(id => id.ToString())
            .ToList();

        List<Game> missingGames = new();
        if (missingSteamIds.Any())
        {
            var imported = await _gameSyncService.ImportByExternalIdsAsync(missingSteamIds, ExternalGameSource.Steam);
            missingGames = imported.ToList();
        }

        return dbGames.Concat(missingGames)
            .Where(g => g.SteamAppId.HasValue)
            .DistinctBy(g => g.SteamAppId!.Value)
            .ToList();
    }


}
