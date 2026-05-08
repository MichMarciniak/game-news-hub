using GameNewsHub.Api.Entities;
using GameNewsHub.Contracts;

namespace GameNewsHub.Api.Features.Genres;

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