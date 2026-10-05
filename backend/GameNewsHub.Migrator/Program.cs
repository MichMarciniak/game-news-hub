using GameNewsHub.Data;
using GameNewsHub.Data.Entities;
using GameNewsHub.Migrator;
using GameNewsHub.Migrator.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

var builder = Host.CreateApplicationBuilder(args);

var config = builder.Configuration;
var services = builder.Services;

var seqOptions = services.AddValidatedOptions<SeqOptions>(config);

builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
    );


builder.Services.AddIdentityCore<AppUser>()
    .AddRoles<IdentityRole<int>>()
    .AddEntityFrameworkStores<AppDbContext>();
    

services.AddSerilog((ctx, cfg) =>
{
    var appName = builder.Environment.ApplicationName;
    
    cfg
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .MinimumLevel.Override("System", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithEnvironmentName()
        .Enrich.WithProperty("Application", appName)
        .WriteTo.Console()
        .WriteTo.File($"../logs/{appName}-.txt",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 14)
        .WriteTo.Seq(seqOptions.Url);
});
    
using var host = builder.Build();
var logger =  host.Services.GetRequiredService<ILogger<Program>>();

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
    
    