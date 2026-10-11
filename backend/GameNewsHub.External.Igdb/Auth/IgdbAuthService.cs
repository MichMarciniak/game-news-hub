using System.Net.Http.Json;
using GameNewsHub.External.Igdb.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GameNewsHub.External.Igdb.Auth;

public class IgdbAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<IgdbAuthService> _logger;
    private readonly IgdbOptions _options;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private volatile string? _cachedToken;
    private DateTime _expiresAt;

    public IgdbAuthService(IOptions<IgdbOptions> config, HttpClient client, ILogger<IgdbAuthService> logger)
    {
        _options = config.Value;
        _httpClient = client;
        _logger = logger;
    }

    //get token
    public async Task<string> GetAccessTokenAsync()
    {
        if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _expiresAt) return _cachedToken;

        await _semaphore.WaitAsync();
        try
        {
            // check again
            if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _expiresAt) return _cachedToken;

            var response = await _httpClient.PostAsync(_options.TokenUrl, new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", _options.ClientId),
                new KeyValuePair<string, string>("client_secret", _options.ClientSecret),
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            }));

            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadFromJsonAsync<TwitchToken>();

            if (data is null || string.IsNullOrEmpty(data.AccessToken))
                throw new Exception("Failed to get Twitch Access Token");

            _cachedToken = data.AccessToken;
            _expiresAt = DateTime.UtcNow.AddSeconds(data.ExpiresIn - 60);

            _logger.LogInformation("Refreshed Twitch access token.");

            return _cachedToken;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}