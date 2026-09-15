using GameNewsHub.Contracts;
using GameNewsHub.Data.Entities;

namespace GameNewsHub.Api.Features.Games;

public static class GameQueryExtension
{
    
    public static IQueryable<Game> ApplySorting(this IQueryable<Game> query, GameSortBy sortBy, bool descending)
    {
        return (sortBy, descending) switch
        {
            (GameSortBy.Name, false) => query.OrderBy(g => g.Title),
            (GameSortBy.Name, true) => query.OrderByDescending(g => g.Title),
            (GameSortBy.Popularity, false) => query.OrderBy(g => g.FollowerCount),
            (GameSortBy.Popularity, true) => query.OrderByDescending(g => g.FollowerCount),
            _ => query.OrderBy(g => g.Title)
        };
    }
    
}