using GameNewsHub.Contracts;
using GameNewsHub.Contracts.Platforms;
using GameNewsHub.Data.Entities;
using Riok.Mapperly.Abstractions;
using System.Linq;

namespace GameNewsHub.Api.Features.Platforms;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class PlatformMapper
{
    [MapperIgnoreTarget(nameof(PlatformGroupWithPlatformsDto.Platforms))]
    public static partial PlatformGroupWithPlatformsDto ToDtoWithPlatforms(PlatformGroup source);

    public static partial PlatformDto ToDto(Platform source);
    public static partial PlatformGroupDto ToDto(PlatformGroup source);
    public static partial IQueryable<PlatformGroupDto> ProjectToDto(this IQueryable<PlatformGroup> query);
}