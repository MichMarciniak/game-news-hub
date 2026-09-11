using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using GameNewsHub.Sync.Sync.Events;

namespace GameNewsHub.Tests.Unit;

public class NormalizeNameTests
{
    public static IEnumerable<object[]> TestCases()
    {
        var path = Path.Combine(
            AppContext.BaseDirectory,
            "resources",
            "test_normalize.json");

        var json = File.ReadAllText(path);

        var cases = JsonSerializer.Deserialize<List<NormalizationTestCase>>(json);

        if (cases == null) throw new Exception("Empty test cases");

        foreach (var testCase in cases) yield return [testCase];
    }

    [Theory]
    [MemberData(nameof(TestCases))]
    public void Should_NormalizeCorrectly(NormalizationTestCase testCase)
    {
        var result = EventNameNormalizer.Normalize(testCase.Name);
        result.Should().Be(testCase.Expected);
    }
}

public class NormalizationTestCase
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    [JsonPropertyName("expected")]
    public string Expected { get; set; } = string.Empty;
}