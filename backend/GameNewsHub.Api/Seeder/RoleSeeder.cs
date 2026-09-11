using GameNewsHub.Api.Constants;
using Microsoft.AspNetCore.Identity;

namespace GameNewsHub.Api.Seeder;

public static class RoleSeeder
{
    public static readonly ICollection<string> _roles = Roles.GetArray();

    public static async Task SeedAsync(RoleManager<IdentityRole<int>> roleManager)
    {
        foreach (var role in _roles)
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<int>(role));
    }
}