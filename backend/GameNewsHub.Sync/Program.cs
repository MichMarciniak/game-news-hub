using GameNewsHub.Data;
using GameNewsHub.External.Igdb;
using GameNewsHub.External.Igdb.Options;
using GameNewsHub.Hosting.Configuration;
using GameNewsHub.Hosting.Logging;
using GameNewsHub.Hosting.Options;
using GameNewsHub.Sync.Core;
using GameNewsHub.Sync.Options;
using GameNewsHub.Sync.Seeder;
using GameNewsHub.Sync.Workers;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

var config = builder.Configuration;
var services = builder.Services;

services.AddValidatedOptions<SyncRangeOptions>(config);
var dbOptions = services.AddValidatedOptions<DatabaseOptions>(config);
var seqOptions = services.AddValidatedOptions<SeqOptions>(config);
var igdbOptions = services.AddValidatedOptions<IgdbOptions>(config);

services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(dbOptions.DefaultConnection)
);

services.AddIgdbServices(igdbOptions);
services.AddSyncCoreServices();
services.AddSyncBackgroundWorkers();

builder.AddAppLogging(seqOptions);

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.MigrateAsync();
    await PlatformGroupSeeder.SeedAsync(context);
}

host.Run();