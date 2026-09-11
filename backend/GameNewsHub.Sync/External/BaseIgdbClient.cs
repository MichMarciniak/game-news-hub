using System.Net.Http.Json;
using GameNewsHub.Sync.Dtos;

namespace GameNewsHub.Sync.External;

public abstract class BaseIgdbClient : IIgdbClient
{
    protected readonly IgdbAuthService AuthService;
    protected readonly HttpClient HttpClient;
    protected readonly ILogger<BaseIgdbClient> Logger;

    protected BaseIgdbClient(HttpClient httpClient, IgdbAuthService authService, ILogger<BaseIgdbClient> logger)
    {
        HttpClient = httpClient;
        AuthService = authService;
        Logger = logger;
    }

    public async Task<List<IgdbGameResponse>> UpdateMissingGames(IEnumerable<int>? gameIds)
    {
        if (gameIds == null || !gameIds.Any()) return new List<IgdbGameResponse>();
        var allGames = new List<IgdbGameResponse>();

        var chunkSize = 150;
        var chunks = gameIds.Chunk(chunkSize);

        foreach (var chunk in chunks)
        {
            var ids = string.Join(',', chunk);
            var query =
                $"fields name, summary, cover.url, genres.name, platforms.name, category, parent_game, game_type.type; " +
                $"where id = ({ids}); limit {chunkSize};";

            var url = "games";

            var result = await SendRequestAsync<IgdbGameResponse>(query, url);
            allGames.AddRange(result);
        }

        return allGames;
    }

    public abstract Task<List<IgdbGameResponse>> GetGamesFromIgdb(int limit = 5);
    public abstract Task<List<IgdbEventResponse>> GetEventsFromIgdb(long from, long to);
    public abstract Task<List<IgdbEventResponse>> UpdateEventsFromIgdb(IEnumerable<int> eventIds);

    protected async Task<HttpRequestMessage> PrepareHttpRequest(string query, string url)
    {
        var token = await AuthService.GetAccessTokenAsync();
        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(query)
        };
        request.Headers.Add("Authorization", $"Bearer {token}");
        return request;
    }

    protected async Task<List<T>> SendRequestAsync<T>(string query, string url)
    {
        var request = await PrepareHttpRequest(query, url);
        Logger.LogInformation($"Sending IGDB request to {url}");

        var response = await HttpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            Logger.LogError(error);
            throw new HttpRequestException($"IGDB API Error: {error}");
        }

        return await response.Content.ReadFromJsonAsync<List<T>>() ?? new List<T>();
    }
}