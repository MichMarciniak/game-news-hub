using GameNewsHub.Data;
using GameNewsHub.Sync;
using GameNewsHub.Sync.Options;
using GameNewsHub.Sync.Seeder;
using GameNewsHub.Sync.Sync;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString)
);

builder.Services.Configure<ApiOptions>(
    builder.Configuration.GetSection(ApiOptions.SectionName));

builder.Services.AddIgdbServices(builder.Configuration);
builder.Services.AddSyncServices();

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.MigrateAsync();
    await PlatformGroupSeeder.SeedAsync(context);
}

host.Run();