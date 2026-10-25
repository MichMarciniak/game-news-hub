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

    public Task<OwnedGameList> GetOwnedGamesAsync(string steamId)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["steamid"] = steamId
        };

        return SendRequestAsync<OwnedGameList>("IPlayerService/GetOwnedGames/v0001/", queryParams);
    }

    public Task<FriendListDto> GetFriendListAsync(string steamId)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["steamid"] = steamId,
            ["relationship"] = "friend"
        };

        return SendRequestAsync<FriendListDto>("ISteamUser/GetFriendList/v0001/", queryParams);
    }

    public Task<WishlistDto> GetWishlistAsync(string steamId)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["steamid"] = steamId
        };

        return SendRequestAsync<WishlistDto>("IWishlistService/GetWishlist/v0001/", queryParams);
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
            return default;
        }

        return await response.Content.ReadFromJsonAsync<T>();
    }
}