using GameNewsHub.Contracts;
using GameNewsHub.Contracts.Platforms;
using GameNewsHub.Data.Entities;

namespace GameNewsHub.Api.Features.Platforms;

public static class PlatformMappings
{
    public static PlatformDto ToDto(this Platform platform)
    {
        return new PlatformDto
        {
            Id = platform.Id,
            Name = platform.Name
        };
    }

    public static PlatformGroupWithPlatformsDto ToDtoWithPlatforms(
        this PlatformGroup group,
        List<PlatformDto> platforms)
    {
        return new PlatformGroupWithPlatformsDto
        {
            Id = group.Id,
            Name = group.Name,
            Platforms = platforms
        };
    }

    public static PlatformGroupDto ToDto(this PlatformGroup group)
    {
        return new PlatformGroupDto
        {
            Id = group.Id,
            Name = group.Name
        };
    }
}