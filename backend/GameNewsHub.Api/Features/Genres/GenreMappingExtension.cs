using GameNewsHub.Contracts.Genres;
using GameNewsHub.Data.Entities;
using Riok.Mapperly.Abstractions;
using System.Linq;

namespace GameNewsHub.Api.Features.Genres;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class GenreMapper
{
    public static partial GenreDto ToDto(Genre source);
    public static partial IQueryable<GenreDto> ProjectToDto(this IQueryable<Genre> query);
}