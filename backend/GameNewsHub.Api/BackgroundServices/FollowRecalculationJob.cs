using GameNewsHub.Data;
using Microsoft.EntityFrameworkCore;

namespace GameNewsHub.Api.Services;

public class FollowRecalculationJob
{
    private readonly AppDbContext _context;

    public FollowRecalculationJob(AppDbContext context)
    {
        _context = context;
    }

    public async Task RecalculateAsync()
    {
        await _context.Database.ExecuteSqlRawAsync(@"
            UPDATE ""Games"" g
            SET ""FollowerCount"" = COALESCE((
                SELECT COUNT(*) 
                FROM ""UserGames"" ug 
                WHERE ug.""FollowedGamesId"" = g.""Id""
            ), 0);
        ");
        
        await _context.Database.ExecuteSqlRawAsync(@"
            UPDATE ""Events"" e
            SET ""FollowerCount"" = COALESCE((
                SELECT COUNT(*) 
                FROM ""UserEvents"" ue 
                WHERE ue.""FollowedEventsId"" = e.""Id""
            ), 0);
        ");
        
    }
}