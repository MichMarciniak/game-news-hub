using GameNewsHub.Data;
using GameNewsHub.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameNewsHub.Sync.Seeder;

public static class PlatformGroupSeeder
{
    public static readonly string[] DefaultGroups =
        { "PlayStation", "Xbox", "Nintendo", "PC", "Mobile", "Other" };

    public static async Task SeedAsync(AppDbContext context)
    {
        var existing = await context.PlatformGroups.Select(g => g.Name).ToListAsync();
        var missing = DefaultGroups.Except(existing).ToList();

        if (missing.Any())
        {
            context.PlatformGroups.AddRange(missing.Select(name => new PlatformGroup { Name = name }));
            await context.SaveChangesAsync();
        }
    }
}