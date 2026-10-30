using GameNewsHub.Contracts.Events;
using GameNewsHub.Data.Entities;

namespace GameNewsHub.Api.Features.Events;

public static class SeriesMappingExtension
{
    public static SeriesDto ToDto(this EventSeries series)
    {
        return new SeriesDto
        {
            Id = series.Id
        };
    }
}