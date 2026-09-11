using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GameNewsHub.Contracts;
using GameNewsHub.Data;
using GameNewsHub.Tests.Integration.Bases;
using GameNewsHub.Tests.Integration.Fixtures;
using GameNewsHub.Tests.Integration.TestData.Events;
using Microsoft.Extensions.DependencyInjection;

namespace GameNewsHub.Tests.Integration.Features.Events;

public record EventListScenario(
    string Description,
    DateTimeOffset StartTime,
    DateTimeOffset? EndTime,
    DateTimeOffset From,
    DateTimeOffset To,
    bool shouldBeIncluded);

public class ReadEventsTests : BaseIntegrationTest
{
    private static readonly DateTimeOffset Anchor = new(2026, 6, 1, 0, 0, 0, TimeSpan.Zero);
    
    public ReadEventsTests(IntegrationTestFixture fixture) : base(fixture)
    {
    }
    
    public static IEnumerable<object[]> EventListScenarios()
    {
        yield return new object[]
        {
            new EventListScenario(
                "Event fully inside range",
                Anchor.AddDays(5), Anchor.AddDays(6),
                Anchor, Anchor.AddDays(10),
                true
            )
        };
        
        yield return new object[]
        {
            new EventListScenario(
                "Event finish after range",
                Anchor.AddDays(5), Anchor.AddDays(11),
                Anchor, Anchor.AddDays(10),
                true
            )
        };
        
        yield return new object[]
        {
            new EventListScenario(
                "Event finish before range",
                Anchor, Anchor.AddDays(1),
                Anchor.AddDays(3), Anchor.AddDays(10),
               false 
            )
        };
        
        yield return new object[]
        {
            new EventListScenario(
                "Event start before range, finish in range",
                Anchor, Anchor.AddDays(5),
                Anchor.AddDays(1), Anchor.AddDays(10),
               true 
            )
        };
        yield return new object[]
        {
            new EventListScenario(
                "Event start before range, finish after range",
                Anchor, Anchor.AddDays(11),
                Anchor.AddDays(1), Anchor.AddDays(10),
                true 
            )
        };
        
        yield return new object[]
        {
            new EventListScenario(
                "Event start at end range",
                Anchor.AddDays(2), Anchor.AddDays(5),
                Anchor, Anchor.AddDays(2),
                true 
            )
        };
        
        yield return new object[]
        {
            new EventListScenario(
                "Event finish at start range",
                Anchor, Anchor.AddDays(1),
                Anchor.AddDays(1), Anchor.AddDays(10),
                true 
            )
        };
        
        yield return new object[]
        {
            new EventListScenario(
                "Event fully outside range",
                Anchor, Anchor.AddDays(1),
                Anchor.AddDays(5), Anchor.AddDays(10),
               false 
            )
        };
        
        yield return new object[]
        {
            new EventListScenario(
                "Event without end, start at end range",
                Anchor.AddDays(2), null,
                Anchor, Anchor.AddDays(2),
                false 
            )
        };
    }

    [Theory]
    [MemberData(nameof(EventListScenarios))]
    public async Task ListEvents_ShouldReturnEvents_WithinDateRange(EventListScenario scenario)
    {
        using var scope = Fixture.Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var evt = new EventBuilder()
            .WithName($"Test Event {Guid.NewGuid():N}")
            .WithStartTime(scenario.StartTime)
            .WithEndTime(scenario.EndTime)
            .Build();

        context.Events.Add(evt);
        await context.SaveChangesAsync();

        var from = Uri.EscapeDataString(scenario.From.ToString());
        var to = Uri.EscapeDataString(scenario.To.ToString());

        var response = await Client.GetAsync($"/Event?from={from}&to={to}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var events = await response.Content.ReadFromJsonAsync<List<EventListItemDto>>();
        events.Should().NotBeNull();
        
        var names = events.Select(e => e.Name).ToList();
        if (scenario.shouldBeIncluded)
            names.Should().Contain(evt.Name);
        else
            names.Should().NotContain(evt.Name);


    }
    
}