using backend.Data;
using GameNewsHub.Api.Entities;
using GameNewsHub.Api.Features.Genres;
using GameNewsHub.Api.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GameNewsHub.Tests;

public class GenreServiceTests : IDisposable, IAsyncDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AppDbContext _context;
    private readonly IGenreService _service;

    public GenreServiceTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();

        _service = new GenreService(_context);
    }

    [Fact]
    public async Task GetOrCreateAsync_WhenNotExists_ShouldCreate()
    {
        //arrange
        var id = 5;
        var name = "test";
        
        //act
        var result = await _service.GetOrCreateAsync(id, name);
        
        //assure
        Assert.NotNull(result);
        Assert.Equal(id, result.IgdbId);
        Assert.Equal(name, result.Name);
    }
    
    [Fact]
    public async Task GetOrCreateAsync_WhenExists_ShouldReturn()
    {
        //arrange
        var existing = new Genre { Id = 1, IgdbId = 24, Name = "Hello" };
        _context.Genres.Add(existing);
        await _context.SaveChangesAsync();
        
        //act
        var result = await _service.GetOrCreateAsync(24, "Hello");
        
        //assure
        Assert.NotNull(result);
        Assert.Equal(24, result.IgdbId);
        Assert.Equal("Hello", result.Name);
    }

    [Theory]
    [InlineData(-1, "RPG")]
    [InlineData(1, null)]
    [InlineData(1, "")]
    public async Task GetOrCreateAsync_WithIncompleteData_ShouldNotAdd(int igdbId, string name)
    {
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetOrCreateAsync(igdbId, name)
            );
    }

    public void Dispose()
    {
        _connection.Close();
        _context.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _connection.DisposeAsync();
        await _context.DisposeAsync();
    }
}