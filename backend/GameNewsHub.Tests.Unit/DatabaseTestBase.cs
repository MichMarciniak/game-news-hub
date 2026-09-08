using backend.Data;

namespace GameNewsHub.Tests;

public class DatabaseTestBase : IDisposable
{
    public AppDbContext Context { get; }

    public DatabaseTestBase()
    {
        Context = TestDbContextFactory.Create();
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}