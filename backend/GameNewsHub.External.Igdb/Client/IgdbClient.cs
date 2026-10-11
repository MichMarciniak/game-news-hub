using System.Net.Http.Json;
using GameNewsHub.External.Igdb.Auth;
using GameNewsHub.External.Igdb.Dtos;
using Microsoft.Extensions.Logging;

namespace GameNewsHub.External.Igdb.Client;

public class IgdbClient : IIgdbClient
{
    private readonly IgdbAuthService _authService;
    private readonly HttpClient _httpClient;
    private readonly ILogger<IgdbClient> _logger;
    
    public IgdbClient(
        HttpClient httpClient, 
        IgdbAuthService authService, 
        ILogger<IgdbClient> logger
        )
    {
        _httpClient = httpClient;
        _authService = authService;
        _logger = logger;
    }
    
    public async Task<List<IgdbDiscoverEventDto>> GetEventsFromIgdb(long from, long to)
    {
        var query = new QueryBuilder()
            .Fields("name, start_time, end_time, description, games.id, live_stream_url")
            .Where($"start_time >= {from} & start_time <= {to}")
            .Sort("start_time", "asc")
            .Limit(500) //domyślny limit to 10, trzeba ustawić go żeby był ponad
            .ToString();

        return await SendRequestAsync<IgdbDiscoverEventDto>(query, "events");
    }

    public async Task<List<IgdbGameDto>> UpdateMissingGames(IEnumerable<int> gameIds)
    {
        var games = await FetchInChunkAsync<int, IgdbGameDto>(gameIds, async chunk =>
        {
            var ids = string.Join(",", chunk);
            var query = new QueryBuilder()
                .Fields(IgdbFields.FullGames)
                .Where($"id = ({ids})")
                .Limit(chunk.Count())
                .ToString();
            
            return await SendRequestAsync<IgdbGameDto>(query, "games");
        }, chunkSize: 200);

        return games;
    }
    public async Task<List<IgdbFullEventDto>> UpdateEventsFromIgdb(IEnumerable<int> eventIds)
    {
        var events = await FetchInChunkAsync<int, IgdbFullEventDto>(eventIds, async chunk =>
        {
            var ids = String.Join(",", chunk);
            var query = new QueryBuilder()
                .Fields("name, start_time, end_time, description, games.id, live_stream_url;")
                .Where($"id = ({ids})")
                .Limit(chunk.Count())
                .ToString();

            return await SendRequestAsync<IgdbFullEventDto>(query, "events");
        }, chunkSize: 200);

        return events;
    }

    // auto chunking magic
    private async Task<List<TResult>> FetchInChunkAsync<TId, TResult>(
        IEnumerable<TId> ids,
        Func<IEnumerable<TId>, Task<List<TResult>>> fetchAction,
        int chunkSize = 200)
    {
        if (ids == null || !ids.Any()) return new List<TResult>();

        var results = new List<TResult>();
        foreach (var chunk in ids.Chunk(chunkSize))
        {
            var chunkResult = await fetchAction(chunk);
            results.AddRange(chunkResult);

            // wait 300ms, żeby nie przekroczyć 4req/s
            await Task.Delay(300);
        }

        return results;
    }

    private async Task<List<T>> SendRequestAsync<T>(string query, string url)
    {
        var token = await _authService.GetAccessTokenAsync();
        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(query)
        };
        request.Headers.Add("Authorization", $"Bearer {token}");
        
        
        _logger.LogInformation($"Sending IGDB request to {url}");
        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError(error);
            throw new HttpRequestException($"IGDB API Error: {error}");
        }

        return await response.Content.ReadFromJsonAsync<List<T>>() ?? new List<T>();
    }
    
    
}