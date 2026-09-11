using GameNewsHub.Contracts;
using GameNewsHub.Data.Entities;

namespace GameNewsHub.Api.Mappings;

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