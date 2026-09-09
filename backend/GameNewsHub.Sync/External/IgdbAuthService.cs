using System.Net.Http.Json;
using backend.Configuration;
using Microsoft.Extensions.Options;

namespace GameNewsHub.Sync.External;

public class IgdbAuthService
{
    private readonly ApiOptions _options;
    private readonly HttpClient _httpClient;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private readonly ILogger<IgdbAuthService> _logger;

    private volatile string? _cachedToken;
    private DateTime _expiresAt;

    public IgdbAuthService(IOptions<ApiOptions> config, HttpClient client, ILogger<IgdbAuthService> logger)
    {
        _options = config.Value;
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
            
            // TODO delete this later, for now it's fine
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