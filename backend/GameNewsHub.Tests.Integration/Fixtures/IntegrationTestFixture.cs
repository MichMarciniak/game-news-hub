using backend.Data;
using DotNet.Testcontainers.Containers;
using GameNewsHub.Sync.Seeder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace GameNewsHub.Tests.Integration;

public class IntegrationTestFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("gamenewshub_test")
        .WithUsername("test")
        .WithPassword("test")
        .Build();

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
                    var descriptor =
                        services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                    if (descriptor is not null) services.Remove(descriptor);

                    services.AddDbContext<AppDbContext>(options =>
                        options.UseNpgsql(ConnectionString));
                });
            });
        
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();


        await context.Database.ExecuteSqlRawAsync("CREATE EXTENSION IF NOT EXISTS pg_trgm;");

        await PlatformGroupSeeder.SeedAsync(context);
    }

    public async Task DisposeAsync()
    {
        await _postgres.DisposeAsync();
        await Factory.DisposeAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        
    }
}

[CollectionDefinition("integration")]
public class IntegrationTestCollection : ICollectionFixture<IntegrationTestFixture> {}