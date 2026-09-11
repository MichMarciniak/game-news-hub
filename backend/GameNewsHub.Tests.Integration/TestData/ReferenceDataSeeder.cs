using Bogus;
using GameNewsHub.Data;
using GameNewsHub.Data.Entities;

namespace GameNewsHub.Tests.Integration.TestData;

public static class ReferenceDataSeeder
{
    public static async Task SeedDataAsync(AppDbContext context)
    {
        var pc = new PlatformGroup { Name = "PC" };
        var ps = new PlatformGroup { Name = "PlayStation" };

        var mac = new Platform { Name = "Mac", PlatformGroup = pc, IgdbId = Faker.GlobalUniqueIndex };
        var windows = new Platform { Name = "Windows", PlatformGroup = pc, IgdbId = Faker.GlobalUniqueIndex };
        var ps5 = new Platform { Name = "PlayStation 5", PlatformGroup = ps, IgdbId = Faker.GlobalUniqueIndex };
        var ps4 = new Platform { Name = "PlayStation 4", PlatformGroup = ps, IgdbId = Faker.GlobalUniqueIndex };

        var rpg = new Genre { Name = "RPG", IgdbId = Faker.GlobalUniqueIndex };
        var adventure = new Genre { Name = "Adventure", IgdbId = Faker.GlobalUniqueIndex };

        context.AddRange(pc, ps, mac, windows, ps5, ps4, rpg, adventure);
        await context.SaveChangesAsync();
    }

    public static class TableNames
    {
        public const string Genres = "Genres";
        public const string Platforms = "Platforms";
        public const string PlatformGroups = "PlatformGroups";
    }
}