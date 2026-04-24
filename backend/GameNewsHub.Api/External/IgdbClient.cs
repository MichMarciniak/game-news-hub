using backend.Configuration;
using backend.Models.DTOs;
using Microsoft.Extensions.Options;

namespace GameNewsHub.Api.External;

public class IgdbClient : IIgdbClient
{
    private readonly HttpClient _httpClient;
    private readonly IgdbAuthService _authService;
    private readonly ApiConfig _config;
    private readonly ILogger<IgdbClient> _logger;

    public IgdbClient(HttpClient httpClient, IgdbAuthService authService, IOptions<ApiConfig> config, ILogger<IgdbClient> logger)
    {
        _httpClient = httpClient;
        _authService = authService;
        _config = config.Value;
        _logger = logger;
    }

    public async Task<List<GameResponse>> GetGamesFromIgdb(int limit = 5)
    {
        var token = await _authService.GetAccessTokenAsync();

        var query = $"fields name, summary, cover.url, genres.name, platforms.name; " +
                    $"limit {limit};";

        var request = new HttpRequestMessage(HttpMethod.Post, "games");
        request.Content = new StringContent(query);
        
        request.Headers.Add("Authorization", $"Bearer {token}");

        _logger.LogInformation(request.ToString());
        
        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"IGDB API Error: {response.StatusCode} - {error} ");
        }

        var games = await response.Content.ReadFromJsonAsync<List<GameResponse>>();
        return games ?? new List<GameResponse>();

    }

    public Task GetEventsFromIgdb(int days = 5)
    {
        throw new NotImplementedException();
    }
}