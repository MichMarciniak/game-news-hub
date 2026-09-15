using FluentAssertions;

namespace GameNewsHub.Tests.Unit;

public class EnumMappingTests
{

    [Theory]
    [InlineData(typeof(Data.Entities.GameType), typeof(Contracts.GameTypeString))]
    public void Enums_ShouldHaveMatchingNamesAndValues(Type dataEnum, Type contractEnum)
    {
        var dataValues = Enum.GetValues(dataEnum)
            .Cast<object>()
            .ToDictionary(v => v.ToString()!, v => (int)v);
        
        var contractValues = Enum.GetValues(contractEnum)
            .Cast<object>()
            .ToDictionary(v => v.ToString()!, v => (int)v);

        dataValues.Should().BeEquivalentTo(contractValues,
            opt => opt.WithStrictOrdering());
    }
}