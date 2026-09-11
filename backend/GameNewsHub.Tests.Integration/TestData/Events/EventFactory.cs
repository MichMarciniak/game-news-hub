using Bogus;
using GameNewsHub.Data.Entities;
using GameNewsHub.Sync.Sync.Events;

namespace GameNewsHub.Tests.Integration.TestData.Events;

public static class EventFactory
{
    public static Faker<Event> Create(Genre[] genres, Game[] games) =>
        new Faker<Event>()
            .RuleFor(e => e.Name, f => f.Commerce.ProductName() + f.IndexGlobal)
            .RuleFor(e => e.NormalizedName, (f, e) => EventNameNormalizer.Normalize(e.Name))
            .RuleFor(e => e.Description, f => f.Lorem.Sentence())
            .RuleFor(e => e.StartTime, f => f.Date.SoonOffset())
            .RuleFor(e => e.Status, f => f.PickRandom<EventSyncStatus>())
            .RuleFor(e => e.IgdbId, f => f.IndexGlobal)
            .RuleFor(e => e.Games, f => f.PickRandom(games, f.Random.Int(0, games.Length)).ToList<Game>())
            .RuleFor(e => e.Series, f => new EventSeries());

}