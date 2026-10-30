using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GameNewsHub.Api.Configuration;
using GameNewsHub.Contracts;
using GameNewsHub.Contracts.Games;
using GameNewsHub.Contracts.Shared;
using GameNewsHub.Data;
using GameNewsHub.Tests.Integration.Bases;
using GameNewsHub.Tests.Integration.Fixtures;
using GameNewsHub.Tests.Integration.TestData.Games;
using Microsoft.Extensions.DependencyInjection;

namespace GameNewsHub.Tests.Integration.Features.Games;

public class ModifyGamesTests : MutatingIntegrationTest
{
    public ModifyGamesTests(IntegrationTestFixture fixture) : base(fixture)
    {
    }

    public static IEnumerable<object[]> SearchTestData()
    {
        var pageSizes = new[] { 10, 25 };
        var queries = new string?[] { null, "Test" };
        var sortOptions = new (GameSortBy? SortBy, bool Descending)[]
        {
            (null, false),
            (GameSortBy.Name, false),
            (GameSortBy.Name, true),
            (GameSortBy.Popularity, false),
            (GameSortBy.Popularity, true)
        };

        foreach (var pageSize in pageSizes)
        {
            foreach (var query in queries)
            {
                foreach (var (sortBy, descending) in sortOptions)
                {
                    yield return new object[] { pageSize, 3, query, sortBy, descending };
                }
            }
        }
    }
    
    [Theory]
    [MemberData(nameof(SearchTestData))]
    public async Task SearchGames_ShouldPage(
        int pageSize, int pages,
        string? query,
        GameSortBy? sortBy, bool descending)
    {
        using var scope = Fixture.Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var totalItems = pageSize * pages;
            
        var games = GameFactory.Create()
            .RuleFor(g => g.Title, f => query != null ?
                $"Test Game {f.UniqueIndex}" : f.Commerce.ProductName())
            .Generate(totalItems);

        context.AddRange(games);
        await context.SaveChangesAsync();

        var fetchedIds = new HashSet<int>();

        for (int page = 1; page <= pages; page++)
        {
            var url = $"/Game/search?pageSize={pageSize}&page={page}" +
                      (query != null ? $"&query={Uri.EscapeDataString(query)}" : "") +
                      (sortBy.HasValue ? $"&sortBy={sortBy.Value}" : "") +
                      $"&descending={descending}";

            var response = await Client.GetAsync(url);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var gameResult = await response.Content.ReadFromJsonAsync<List<GameListItemDto>>();
            gameResult.Should().NotBeNull();
            gameResult.Should().HaveCountLessThanOrEqualTo(pageSize);

            foreach (var game in gameResult!)
            {
                // hash set od razu zwraca true jeśli nie istnieje
                fetchedIds.Add(game.Id).Should().BeTrue();
            }
        }

        fetchedIds.Should().HaveCount(totalItems); 

    }
}