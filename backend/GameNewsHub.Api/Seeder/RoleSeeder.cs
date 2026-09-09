using Microsoft.AspNetCore.Identity;

namespace GameNewsHub.Api.Seeder;

public static class RoleSeeder
{
    public static readonly string[] Roles = new[] { "Admin", "User" };

    public static async Task SeedAsync(RoleManager<IdentityRole<int>> roleManager)
    {
        foreach (var role in Roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<int>(role));
            }
            
        }
    }
}