using backend.Configuration;
using backend.Models.DTOs;
using Microsoft.Extensions.Options;

namespace backend.Services.Background;

public class IgdbAuthService
{
    private readonly ApiConfig _config;
    private readonly HttpClient _httpClient;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private readonly ILogger<IgdbAuthService> _logger;

    private string? _cachedToken;
    private DateTime _expiresAt;

    public IgdbAuthService(IOptions<ApiConfig> config, HttpClient client, ILogger<IgdbAuthService> logger)
    {
        _config = config.Value;
        _httpClient = client;
        _logger = logger;
    }
    
    //get token
    public async Task<string> GetAccessTokenAsync()
    {
        if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _expiresAt)
        {
            return _cachedToken;
        }

        await _semaphore.WaitAsync();
        try
        {
            // check again
            if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _expiresAt)
            {
                return _cachedToken;
            }

            var response = await _httpClient.PostAsync(_config.TokenUrl, new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", _config.ClientId),
                new KeyValuePair<string, string>("client_secret", _config.ClientSecret),
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            }));

            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadFromJsonAsync<TwitchToken>();

            if (data is null || string.IsNullOrEmpty(data.AccessToken))
                throw new Exception("Failed to get Twitch Access Token");

            _cachedToken = data.AccessToken;
            _expiresAt = DateTime.UtcNow.AddSeconds(data.ExpiresIn - 60);
            
            _logger.LogInformation("Refreshed Twitch access token.");
            _logger.LogInformation(data.AccessToken);
            _logger.LogInformation(_cachedToken);

            return _cachedToken;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}