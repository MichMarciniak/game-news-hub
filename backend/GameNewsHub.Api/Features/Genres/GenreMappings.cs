using GameNewsHub.Api.Dtos;
using GameNewsHub.Data.Entities;

namespace GameNewsHub.Api.Features.Genres;

public static class GenreMappings
{
    public static GenreResponse ToDto(this Genre genre)
    {
        return new GenreResponse
        {
            Id = genre.IgdbId,
            Name = genre.Name
        };
    }
}