using backend.Models.DTOs;
using backend.Models.Entities;

namespace GameNewsHub.Api.Mappings;

public static class GenreMappings
{
    public static GenreDto ToDto(this Genre genre)
    {
        return new GenreDto
        {
            Id = genre.IgdbId,
            Name = genre.Name
        };
    }
}