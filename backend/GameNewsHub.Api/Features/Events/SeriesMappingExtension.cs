using GameNewsHub.Contracts.Events;
using GameNewsHub.Data.Entities;
using Riok.Mapperly.Abstractions;

namespace GameNewsHub.Api.Features.Events;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class SeriesMapper
{
    public static partial SeriesDto ToDto(EventSeries source);

    [MapperIgnoreTarget(nameof(AdminSeriesDto.DisplayName))]
    [MapperIgnoreTarget(nameof(AdminSeriesDto.Events))]
    public static partial AdminSeriesDto ToAdminDto(EventSeries source);
}