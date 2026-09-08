using FluentAssertions;
using GameNewsHub.Sync.Sync.Events;

namespace GameNewsHub.Tests;

public class NormalizeNameTests 
{

    // jakoś wczytać z jsona?
    [Theory]
    [InlineData("SAGE 2025","SAGE")]
    [InlineData("Fallout Day 2025","Fallout Day")]
    [InlineData("Galaxies Autumn '25","Galaxies")]
    [InlineData("  Test 123  321 123 321   ","Test")]
    [InlineData("State of Play | September 24, 2025","State of Play")]
    [InlineData("Indie Fan Fest: Fall 2025","Indie Fan Fest")]
    [InlineData("Beyond the Strand","Beyond the Strand")]
    [InlineData("MIDSUMMER NIGHT'S SCREAM:","MIDSUMMER NIGHT'S SCREAM")]
    [InlineData("Shacknews E4 Indie Showcase 2026","Shacknews E4 Indie Showcase")]
    public void TestNormalize(string name, string expected)
    {
        var value = EventNameNormalizer.Normalize(name);

        value.Should().Be(expected);
    } 
    
}