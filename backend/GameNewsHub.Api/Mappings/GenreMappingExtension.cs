using GameNewsHub.Contracts;
using GameNewsHub.Data.Entities;

namespace GameNewsHub.Api.Mappings;

public static class GenreMappingExtension
{
    public static GenreDto ToDto(this Genre genre)
    {
        return new GenreDto
        {
            Id = genre.Id,
            Name = genre.Name
        };
    }
}