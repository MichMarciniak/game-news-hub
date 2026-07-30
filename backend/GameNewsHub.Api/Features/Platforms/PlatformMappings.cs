using GameNewsHub.Api.Dtos;
using GameNewsHub.Data.Entities;

namespace GameNewsHub.Api.Features.Platforms;

public static class PlatformMappings
{
    public static PlatformResponse ToDto(this Platform platform)
    {
        return new PlatformResponse 
        {
            Id = platform.Id,
            Name = platform.Name
        };
    }
}