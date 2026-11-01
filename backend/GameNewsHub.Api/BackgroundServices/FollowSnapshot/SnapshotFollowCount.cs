using GameNewsHub.Data;
using Microsoft.EntityFrameworkCore;

namespace GameNewsHub.Api.Services.FollowSnapshot;

public class SnapshotFollowCount
{
    private readonly AppDbContext _context;

    public SnapshotFollowCount(AppDbContext context)
    {
        _context = context;
    }

    public async Task SnapshotFollowerCount()
    {
        var now = DateTimeOffset.UtcNow;
        await _context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO ""EventFollowerSnapshot"" (""EventId"", ""FollowerCount"", ""RecordedAt"")
            SELECT e.""Id"", COUNT(ue.""AppUserId""), {0}
            FROM ""Events"" e
            LEFT JOIN ""UserEvents"" ue ON ue.""FollowedEventsId"" = e.""Id""
            GROUP BY e.""Id"";
        ", now);
    }
}