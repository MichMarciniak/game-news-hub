using GameNewsHub.External.Igdb.Dtos;

namespace GameNewsHub.External.Igdb.Client;

public interface IIgdbClient
{
    public Task<List<IgdbGameDto>> UpdateMissingGames(IEnumerable<int> gameIds);

    public Task<List<IgdbDiscoverEventDto>> GetEventsFromIgdb(long from, long to);

    public Task<List<IgdbFullEventDto>> UpdateEventsFromIgdb(IEnumerable<int> eventIds);
}