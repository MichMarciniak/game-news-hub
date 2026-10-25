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

}
