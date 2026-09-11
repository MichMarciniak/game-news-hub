using Bogus;
using GameNewsHub.Data.Entities;

namespace GameNewsHub.Tests.Integration.TestData.Games;

public class GameFactory
{
    public static Faker<Game> Create(Genre[] genres, Platform[] platforms) =>
        new Faker<Game>()
            .RuleFor(g => g.Title, f => f.Commerce.ProductName())
            .RuleFor(g => g.Summary, f => f.Lorem.Sentence())
            .RuleFor(g => g.IgdbId, f => f.IndexGlobal)
            .RuleFor(g => g.Platforms, f => f.PickRandom(platforms, f.Random.Int(1, 2)).ToList())
            .RuleFor(g => g.Genres, f => f.PickRandom(genres, f.Random.Int(1, 3)).ToList());

}