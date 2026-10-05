using GameNewsHub.Data;
using GameNewsHub.Sync;
using GameNewsHub.Sync.Options;
using GameNewsHub.Sync.Seeder;
using GameNewsHub.Sync.Sync;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

var builder = Host.CreateApplicationBuilder(args);

var config = builder.Configuration;
var services = builder.Services;

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var seqOptions = services.AddValidatedOptions<SeqOptions>(config);

services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString)
);

services.Configure<ApiOptions>(
    config.GetSection(ApiOptions.SectionName));

services.AddIgdbServices(builder.Configuration);
services.AddSyncServices();


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

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.MigrateAsync();
    await PlatformGroupSeeder.SeedAsync(context);
}

host.Run();