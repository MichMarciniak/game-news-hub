using GameNewsHub.Api.Sync.Events;
using GameNewsHub.Api.Sync.Games;
using GameNewsHub.Contracts;

namespace GameNewsHub.Api.External;

public interface IIgdbClient
{
    public Task<List<GameRequest>> GetGamesFromIgdb(int limit = 5);
    public Task<List<GameRequest>> UpdateMissingGames(IEnumerable<int> gameIds);

    public Task<List<EventResponse>> GetEventsFromIgdb(long from, long to);

    public Task<List<EventResponse>> UpdateEventsFromIgdb(IEnumerable<int> eventIds);
}