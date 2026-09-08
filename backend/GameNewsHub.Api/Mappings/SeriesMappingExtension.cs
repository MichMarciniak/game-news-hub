using Data.Entities;
using GameNewsHub.Contracts;

namespace GameNewsHub.Api.Mappings;

public static class SeriesMappingExtension
{
    public static SeriesDto ToDto(this EventSeries series)
    {
        return new SeriesDto
        {
            Id = series.Id,
            Name = series.Name,
        };
    }
}