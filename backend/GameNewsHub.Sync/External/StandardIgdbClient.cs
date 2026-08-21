using System.Net.Http.Json;
using backend.Configuration;
using GameNewsHub.Sync.Dtos;
using Microsoft.Extensions.Options;

namespace GameNewsHub.Sync.External;

public class StandardIgdbClient : BaseIgdbClient, IIgdbClient 
{

    public StandardIgdbClient(HttpClient httpClient, IgdbAuthService authService, ILogger<BaseIgdbClient> logger) : base(httpClient, authService, logger)
    {
    }

    public override async Task<List<IgdbGameResponse>> GetGamesFromIgdb(int limit = 5)
    {
        var query = $"fields name, summary, cover.url, genres.name, platforms.name; " +
                    $"limit {limit};";
        var url = "games";
        return await SendRequestAsync<IgdbGameResponse>(query, url);
    }


    public override async Task<List<IgdbEventResponse>> GetEventsFromIgdb(long from, long to)
    {
        var query = $"fields name, start_time, end_time, description, games.id; " + 
                    $"where start_time >= {from} & start_time <= {to};" +
                    $"sort start_time asc;";
        var url = "events";
        return await SendRequestAsync<IgdbEventResponse>(query, url);
    }

    public override async Task<List<IgdbEventResponse>> UpdateEventsFromIgdb(IEnumerable<int> eventIds)
    {
        var ids = string.Join(",", eventIds);
        var query = $"fields name, start_time, end_time, description, games.id;" +
                    $"where id = ({ids});";
        var url = "events";
        return await SendRequestAsync<IgdbEventResponse>(query, url);
    }
}