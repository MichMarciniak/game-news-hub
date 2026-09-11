using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GameNewsHub.Contracts;
using GameNewsHub.Data;
using GameNewsHub.Tests.Integration.Bases;
using GameNewsHub.Tests.Integration.Fixtures;
using GameNewsHub.Tests.Integration.TestData.Games;
using Microsoft.Extensions.DependencyInjection;

namespace GameNewsHub.Tests.Integration.Features.Games;

public record GameSearchScenario(string Query, string[] ExpectedTitles, string[] UnexpectedTitles);

public class ReadGamesTests : BaseIntegrationTest
{
    public ReadGamesTests(IntegrationTestFixture fixture) : base(fixture)
    {
    }

    public static IEnumerable<object[]> SearchScenarios()
    {
        yield return new object[]
        {
            new GameSearchScenario(
                "witcher",
                new[] { "The Witcher 3" },
                new[] { "Cyberpunk 2077" })
        };

        yield return new object[]
        {
            new GameSearchScenario(
                "wiedźmin",
                Array.Empty<string>(),
                new[] { "Cyberpunk 2077, The Witcher 3" })
        };
        
    }

    [Theory]
    [MemberData(nameof(SearchScenarios))]
    public async Task SearchGames_ShouldReturnGames(GameSearchScenario scenario)
    {
        using var scope = Fixture.Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        var witcher3 = new GameBuilder().WithTitle("The Witcher 3").Build();
        var cp = new GameBuilder().WithTitle("Cyberpunk 2077").Build();
        
        context.AddRange(witcher3, cp);
        await context.SaveChangesAsync();
        
        var response = await Client.GetAsync($"Game/search?query={Uri.EscapeDataString(scenario.Query)}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var games = await response.Content.ReadFromJsonAsync<List<GameListItemDto>>();
        games.Should().NotBeNull();

        var titles = games.Select(g => g.Title).ToList();
        if (scenario.ExpectedTitles.Any())
            titles.Should().Contain(scenario.ExpectedTitles);
        else titles.Should().BeEmpty();
        
        titles.Should().NotContain(scenario.UnexpectedTitles);
    }
    
    
}