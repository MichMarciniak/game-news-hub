using Data.Entities;
using GameNewsHub.Contracts;
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

    public static PlatformGroupDto ToDtoWithPlatforms(this PlatformGroup group)
    {
        return new PlatformGroupDto
        {
            Id = group.Id,
            Name = group.Name,
            Platforms = group.Platforms.Select(ToDto).ToList()
        };
    }

    public static PlatformGroupDto ToDto(this PlatformGroup group)
    {
        return new PlatformGroupDto
        {
            Id = group.Id,
            Name = group.Name,
        };
    }

}