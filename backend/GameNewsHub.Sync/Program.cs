using backend.Configuration;
using backend.Data;
using GameNewsHub.Sync.Sync;
using GameNewsHub.Sync;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString)
);

builder.Services.Configure<ApiConfig>(
    builder.Configuration.GetSection("Api"));

builder.Services.AddIgdbServices(builder.Configuration);
builder.Services.AddSyncServices();

var host = builder.Build();
host.Run();