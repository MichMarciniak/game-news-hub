using GameNewsHub.Sync.Dtos;

namespace GameNewsHub.Sync.External;

public class BulkIgdbClient : BaseIgdbClient, IIgdbClient
{
    public BulkIgdbClient(HttpClient httpClient, IgdbAuthService authService, ILogger<BaseIgdbClient> logger) : base(httpClient, authService, logger)
    {
    }


    public override Task<List<IgdbGameResponse>> GetGamesFromIgdb(int limit = 5)
    {
        throw new NotImplementedException();
    }

    public override async Task<List<IgdbEventResponse>> GetEventsFromIgdb(long from, long to)
    {
        var query = $"fields name, start_time, end_time, description, games.id; " + 
                    $"where start_time >= {from} & start_time <= {to};" +
                    $"sort start_time asc; limit 500;";
        var url = "events";
        return await SendRequestAsync<IgdbEventResponse>(query, url);
    }

    public override async Task<List<IgdbEventResponse>> UpdateEventsFromIgdb(IEnumerable<int> eventIds)
    {
        var ids = string.Join(",", eventIds);
        var query = $"fields name, start_time, end_time, description, games.id;" +
                    $"where id = ({ids}); limit 500;";
        var url = "events";
        return await SendRequestAsync<IgdbEventResponse>(query, url);
    }
}