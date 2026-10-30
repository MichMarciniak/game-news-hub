using GameNewsHub.Contracts.Genres;
using GameNewsHub.Data.Entities;

namespace GameNewsHub.Api.Features.Genres;

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