using GameNewsHub.Data;

namespace GameNewsHub.Tests.Unit;

public class DatabaseTestBase : IDisposable
{
    public DatabaseTestBase()
    {
        Context = TestDbContextFactory.Create();
    }

    public AppDbContext Context { get; }

    public void Dispose()
    {
        Context.Dispose();
    }
}