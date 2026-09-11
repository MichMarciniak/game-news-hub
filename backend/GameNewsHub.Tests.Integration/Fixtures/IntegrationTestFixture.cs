using System.Data.Common;
using GameNewsHub.Data;
using GameNewsHub.Sync.Seeder;
using GameNewsHub.Tests.Integration.TestData;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;

namespace GameNewsHub.Tests.Integration.Fixtures;

public class IntegrationTestFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("gamenewshub_test")
        .WithUsername("test")
        .WithPassword("test")
        .Build();

    private DbConnection _connection = default!;
    private Respawner _respawner = default!;

    public WebApplicationFactory<Program> Factory { get; private set; } = null!;
    public string ConnectionString => _postgres.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        Factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d =>
                        d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                    if (descriptor is not null)
                        services.Remove(descriptor);

                    services.AddDbContext<AppDbContext>(options =>
                        options.UseNpgsql(ConnectionString));
                });
            });

        using (var scope = Factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await context.Database.MigrateAsync();

            await context.Database.ExecuteSqlRawAsync("CREATE EXTENSION IF NOT EXISTS pg_trgm;");

            await SeedInitialDataAsync(context);
        }

        _connection = new NpgsqlConnection(ConnectionString);
        await _connection.OpenAsync();

        _respawner = await Respawner.CreateAsync(_connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            TablesToIgnore =
            [
                "__EFMigrationsHistory",
                ReferenceDataSeeder.TableNames.Genres,
                ReferenceDataSeeder.TableNames.Platforms,
                ReferenceDataSeeder.TableNames.PlatformGroups
            ]
        });
    }

    public async Task DisposeAsync()
    {
        if (_connection is not null)
        {
            await _connection.CloseAsync();
            await _connection.DisposeAsync();
        }

        await _postgres.DisposeAsync();
        await Factory.DisposeAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_connection);

        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await SeedInitialDataAsync(context);
    }

    private async Task SeedInitialDataAsync(AppDbContext context)
    {
        await PlatformGroupSeeder.SeedAsync(context);
    }
}