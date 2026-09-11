using Bogus;
using GameNewsHub.Data.Entities;

namespace GameNewsHub.Tests.Integration.TestData.Games;

public class GameBuilder
{
    private static readonly Faker Faker = new();
    private readonly List<Genre> _genres = new();
    private readonly List<Platform> _platforms = new();
    private int _igdbId = Faker.IndexGlobal;
    private Game _parentGame;
    private string _summary = Faker.Lorem.Paragraph();

    private string _title = Faker.Commerce.ProductName();
    private GameType _type = GameType.MainGame;

    public GameBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public GameBuilder WithSummary(string summary)
    {
        _summary = summary;
        return this;
    }

    public GameBuilder WithType(GameType type)
    {
        _type = type;
        return this;
    }

    public GameBuilder WithPlatforms(List<Platform> platforms)
    {
        _platforms.AddRange(platforms);
        return this;
    }

    public GameBuilder WithGenres(List<Genre> genres)
    {
        _genres.AddRange(genres);
        return this;
    }

    public GameBuilder WithIgdbId(int id)
    {
        _igdbId = id;
        return this;
    }

    public GameBuilder WithParentGame(Game parentGame)
    {
        _parentGame = parentGame;
        return this;
    }

    public Game Build() => new()
    {
        Title = _title,
        Summary = _summary,
        Type = _type,
        Platforms = _platforms,
        IgdbId = _igdbId,
        Genres = _genres,
        ParentGame = _parentGame
    };
}