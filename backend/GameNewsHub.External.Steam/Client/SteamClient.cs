using System.Net.Http.Json;
using GameNewsHub.External.Steam.Dtos;
using GameNewsHub.External.Steam.Options;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GameNewsHub.External.Steam.Client;

public class SteamClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SteamClient> _logger;
    private readonly SteamOptions _steamOptions;

    public SteamClient(
        HttpClient httpClient,
        ILogger<SteamClient> logger,
        IOptions<SteamOptions> steamOptions
        )
    {
        _httpClient = httpClient;
        _logger = logger;
        _steamOptions = steamOptions.Value;
    }
    public async Task<OwnedGameList?> GetSteamOwnedGames(ulong steamUserId)
    {
        var key = _steamOptions.Key;
        var parameters = new Dictionary<string, string>
        {
            { "key", key },
            { "steamid", steamUserId.ToString() },
            { "format", "json" },
            { "include_played_free_games", "true" }
        };
        
        var baseUrl = "IPlayerService/GetOwnedGames/v0001";
        var url = QueryHelpers.AddQueryString(baseUrl, parameters!);

        _logger.LogInformation("Sending steam request for user id: {steamUserId}", steamUserId);
        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError(error);
            throw new HttpRequestException($"Steam api error");
        }

        var result = await response.Content.ReadFromJsonAsync<SteamGamesResponse>();
        return result.Response;
    }

    public async Task LoginToSteam()
    {
        
    }
    
}