using System.Net.Http.Json;
using GameNewsHub.External.Steam.Dtos;
using GameNewsHub.External.Steam.Options;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GameNewsHub.External.Steam.Client;

public class SteamApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SteamApiClient> _logger;
    private readonly SteamOptions _steamOptions;

    public SteamApiClient(
        HttpClient httpClient,
        ILogger<SteamApiClient> logger,
        IOptions<SteamOptions> steamOptions
        )
    {
        _httpClient = httpClient;
        _logger = logger;
        _steamOptions = steamOptions.Value;
    }

    public Task<SteamGamesResponse> GetOwnedGamesAsync(string steamId)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["steamid"] = steamId
        };

        return SendRequestAsync<SteamGamesResponse>("IPlayerService/GetOwnedGames/v0001/", queryParams);
    }

    public Task<FriendListResponse> GetFriendListAsync(string steamId)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["steamid"] = steamId,
            ["relationship"] = "friend"
        };

        return SendRequestAsync<FriendListResponse>("ISteamUser/GetFriendList/v0001/", queryParams);
    }

    public Task<WishlistResponse> GetWishlistAsync(string steamId)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["steamid"] = steamId
        };

        return SendRequestAsync<WishlistResponse>("IWishlistService/GetWishlist/v0001/", queryParams);
    }
    
    private async Task<T> SendRequestAsync<T>(
        string relativeUrl,
        Dictionary<string, string>? customParams = null
    )
    {
        var queryParams = new Dictionary<string, string>
        {
            ["key"] = _steamOptions.Key
        };

        if (customParams != null)
        {
            foreach (var (key, value) in customParams)
            {
                queryParams.Add(key, value);
            }
        }

        var requestUrl = QueryHelpers.AddQueryString(relativeUrl, queryParams);

        var response = await _httpClient.GetAsync(requestUrl);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError(error);
            throw new HttpRequestException("Steam api error");
        }

        return await response.Content.ReadFromJsonAsync<T>();
    }
}