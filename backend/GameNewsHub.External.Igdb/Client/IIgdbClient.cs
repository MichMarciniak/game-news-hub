using GameNewsHub.External.Igdb.Dtos;

namespace GameNewsHub.External.Igdb.Client;

public interface IIgdbClient
{
    public Task<List<IgdbGameResponse>> GetGamesFromIgdb(int limit = 5);
    public Task<List<IgdbGameResponse>> UpdateMissingGames(IEnumerable<int> gameIds);

    public Task<List<IgdbEventResponse>> GetEventsFromIgdb(long from, long to);

    public Task<List<IgdbEventResponse>> UpdateEventsFromIgdb(IEnumerable<int> eventIds);
}