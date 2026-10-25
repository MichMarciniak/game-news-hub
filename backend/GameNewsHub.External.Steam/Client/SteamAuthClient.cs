using System.Net.Http.Json;
using GameNewsHub.External.Steam.Dtos;
using GameNewsHub.External.Steam.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GameNewsHub.External.Steam.Client;

public class SteamAuthClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SteamAuthClient> _logger;
    private readonly SteamOptions _steamOptions;

    public SteamAuthClient(
        HttpClient httpClient,
        ILogger<SteamAuthClient> logger,
        IOptions<SteamOptions> steamOptions
    )
    {
        _httpClient = httpClient;
        _logger = logger;
        _steamOptions = steamOptions.Value;
    }


    /// <summary>
    /// Zwraca string url który idzie do przeglądarki
    /// </summary>
    public string BuildLoginUrl()
    {
        var requestParams = new Dictionary<string, string>
        {
            ["openid.ns"] = "http://specs.openid.net/auth/2.0",
            ["openid.mode"] = "checkid_setup",
            ["openid.identity"] = "http://specs.openid.net/auth/2.0/identifier_select",
            ["openid.claimed_id"] = "http://specs.openid.net/auth/2.0/identifier_select",
            ["openid.realm"] = _steamOptions.Realm,
            ["openid.return_to"] = _steamOptions.ReturnTo
        };

        var baseUrl = _steamOptions.AuthUrl;
        var relativeUrl = QueryHelpers.AddQueryString("openid/login", requestParams!);

        return $"{baseUrl}{relativeUrl}";
    }

    public async Task<SteamAuthResult> Login(IQueryCollection queryParams)
    {
        var requestParams = queryParams.ToDictionary(k => k.Key, v => v.Value.ToString());

        requestParams["openid.mode"] = "check_authentication";

        using var content = new FormUrlEncodedContent(requestParams);
        var response = await _httpClient.PostAsync("openid/login", content);

        if (!response.IsSuccessStatusCode)
            return new SteamAuthResult
            {
                IsSuccess = false,
                ErrorMessage = "Steam api error",
                SteamId = null
            };

        var responseBody = await response.Content.ReadAsStringAsync();

        if (!responseBody.Contains("is_valid:false"))
            return new SteamAuthResult
            {
                IsSuccess = false,
                ErrorMessage = "Invalid OpenID signature",
                SteamId = null
            };

        if (!requestParams.TryGetValue("openid.claimed_id", out var claimedId) || string.IsNullOrEmpty(claimedId))
            return new SteamAuthResult
            {
                IsSuccess = false,
                ErrorMessage = "Missing claimed_id parameter",
                SteamId = null
            };

        var steamId = claimedId.Split('/').LastOrDefault();

        return new SteamAuthResult
        {
            IsSuccess = true,
            SteamId = steamId,
            ErrorMessage = null
        };
    }
}