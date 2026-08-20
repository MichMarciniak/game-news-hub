using System.Net.Http.Json;
using backend.Configuration;
using GameNewsHub.Sync.Dtos;
using Microsoft.Extensions.Options;

namespace GameNewsHub.Sync.External;

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

    private async Task<HttpRequestMessage> PrepareHttpRequest(string query, string url)
    {
        var token = await _authService.GetAccessTokenAsync();

        var request = new HttpRequestMessage(HttpMethod.Post, url); 
        request.Content = new StringContent(query);
        
        request.Headers.Add("Authorization", $"Bearer {token}");

        return request;
    }

    private async Task<List<T>> SendRequestAsync<T>(string query, string url)
    {
        var request = await PrepareHttpRequest(query, url);

        _logger.LogInformation($"Sending IGDB request to {url} with query: {query}");

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await request.Content.ReadAsStringAsync();
            _logger.LogError($"IGDB API Error: {response.StatusCode} - {error}");
            throw new Exception($"IGDB API Error: {response.StatusCode} - {error} ");
        }

        return await response.Content.ReadFromJsonAsync<List<T>>() ?? new List<T>();
    }

    public async Task<List<IgdbGameResponse>> GetGamesFromIgdb(int limit = 5)
    {
        var query = $"fields name, summary, cover.url, genres.name, platforms.name; " +
                    $"limit {limit};";
        var url = "games";
        return await SendRequestAsync<IgdbGameResponse>(query, url);
    }

    public async Task<List<IgdbGameResponse>> UpdateMissingGames(IEnumerable<int> gameIds)
    {
        var ids = string.Join(',', gameIds);
        var query = $"fields name, summary, cover.url, genres.name, platforms.name, category, parent_game; " +
                    $"where id = ({ids});";
        var url = "games";
        return await SendRequestAsync<IgdbGameResponse>(query, url);
    }

    public async Task<List<IgdbEventResponse>> GetEventsFromIgdb(long from, long to)
    {
        var query = $"fields name, start_time, end_time, description, games.id; " + 
                    $"where start_time >= {from} & start_time <= {to};" +
                    $"sort start_time asc; limit 500;";
        var url = "events";
        return await SendRequestAsync<IgdbEventResponse>(query, url);
    }

    public async Task<List<IgdbEventResponse>> UpdateEventsFromIgdb(IEnumerable<int> eventIds)
    {
        var ids = string.Join(",", eventIds);
        var query = $"fields name, start_time, end_time, description, games.id;" +
                    $"where id = ({ids}); limit 500;";
        var url = "events";
        return await SendRequestAsync<IgdbEventResponse>(query, url);
    }
}