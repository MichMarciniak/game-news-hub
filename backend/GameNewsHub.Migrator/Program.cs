using GameNewsHub.Data;
using GameNewsHub.Data.Entities;
using GameNewsHub.Hosting.Configuration;
using GameNewsHub.Hosting.Logging;
using GameNewsHub.Hosting.Options;
using GameNewsHub.Migrator;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

var config = builder.Configuration;
var services = builder.Services;

var seqOptions = services.AddValidatedOptions<SeqOptions>(config);
var dbOptions = services.AddValidatedOptions<DatabaseOptions>(config);

builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseNpgsql(dbOptions.DefaultConnection)
);

builder.Services.AddIdentityCore<AppUser>()
    .AddRoles<IdentityRole<int>>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.AddAppLogging(seqOptions);
    
using var host = builder.Build();
var logger = host.Services.GetRequiredService<ILogger<Program>>();

try
{
    using var scope = host.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

    logger.LogInformation("Applying migrations");
    await db.Database.MigrateAsync();
    
    logger.LogInformation("Seeding roles");
    await RoleSeeder.SeedAsync(roleManager);
    
    logger.LogInformation("Done");
    
    return 0;
}
catch (Exception ex)
{
    logger.LogCritical(ex, "Migration failed");
    return 1;
}
    
    