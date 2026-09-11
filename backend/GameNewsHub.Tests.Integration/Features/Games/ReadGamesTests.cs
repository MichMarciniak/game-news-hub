using System.Net;
using System.Net.Http.Json;
using backend.Data;
using FluentAssertions;
using GameNewsHub.Contracts;
using GameNewsHub.Data.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace GameNewsHub.Tests.Integration;

[Collection("Integration")]
public class GetGamesTests
{
    private readonly IntegrationTestFixture _fixture;

    public GetGamesTests(IntegrationTestFixture fixture) => _fixture = fixture;

    public async Task InitializeAsync()
    {
        using var scope = _fixture.Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var genre = new Genre { Name = "RPG", IgdbId = 1 };
        var game = new Game { Title = "Test Game", IgdbId = 100, Genres = { genre } };
        var user = new AppUser { UserName = "testuser", FollowedGames = { game } };
        var evt = new Event
        {
            Name = "Test Event",
            IgdbId = 500,
            StartTime = DateTimeOffset.UtcNow.AddDays(-5),
            Games = { game },
            Status = EventSyncStatus.Ready
        };
        
        context.AddRange(genre, game, user, evt);
        await context.SaveChangesAsync();
    }
    
    
    [Fact]
    public async Task GetGames_ReturnGames()
    {
        var client = _fixture.Factory.CreateClient();

        var response = await client.GetAsync("/Game/list");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var games = await response.Content.ReadFromJsonAsync<List<GameListItemDto>>();
        games.Should().NotBeEmpty();
    }

}