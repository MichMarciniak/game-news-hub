using Bogus;
using GameNewsHub.Data.Entities;

namespace GameNewsHub.Tests.Integration.TestData.Events;

public class EventBuilder
{
    private static int _idCounter = 1;
    private int _igdbId = Interlocked.Increment(ref _idCounter);
    
    private static readonly Faker Faker = new();

    private string _name = Faker.Commerce.ProductName();
    private string? _normalizedName;
    private string _description = Faker.Lorem.Paragraph();
    private DateTimeOffset _startTime;
    private DateTimeOffset? _endTime;
    private EventSyncStatus _status = EventSyncStatus.NoData;
    private readonly List<Game> _games = new();
    private readonly List<EventGenreWeight> _genreWeights = new();
    private EventSeries? _series;

    public EventBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public EventBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }
    
    public EventBuilder WithStartTime(DateTimeOffset startTime)
    {
        _startTime = startTime;
        return this;
    }

    public EventBuilder WithEndTime(DateTimeOffset? endTime)
    {
        _endTime = endTime;
        return this;
    }

    public EventBuilder WithStatus(EventSyncStatus status)
    {
        _status = status;
        return this;
    }

    public EventBuilder WithIgdbId(int igdbId)
    {
        _igdbId = igdbId;
        return this;
    }

    public EventBuilder WithGames(List<Game> games)
    {
        _games.AddRange(games);
        return this;
    }

    public EventBuilder WithGenreWeight(Genre genre, double weight)
    {
        _genreWeights.Add(new EventGenreWeight { GenreId = genre.Id, Weight = weight});
        return this;
    }

    public EventBuilder WithSeries(EventSeries series)
    {
        _series = series;
        return this;
    }

    public Event Build() => new()
    {
        Name = _name,
        NormalizedName = _normalizedName ?? _name,
        Description = _description,
        StartTime = _startTime,
        EndTime = _endTime,
        Status = _status,
        IgdbId = _igdbId,
        Games = _games,
        GenreWeights = _genreWeights,
        Series = _series ?? new EventSeries()
    };

}