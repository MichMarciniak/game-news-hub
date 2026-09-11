using System.Text.Json;
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
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Expected { get; set; } = string.Empty;
}