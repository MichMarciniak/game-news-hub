using GameNewsHub.Tests.Integration.Fixtures;

namespace GameNewsHub.Tests.Integration.Bases;

public abstract class MutatingIntegrationTest : BaseIntegrationTest, IAsyncLifetime
{
    protected MutatingIntegrationTest(IntegrationTestFixture fixture) : base(fixture)
    {
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await Fixture.ResetDatabaseAsync();
    }
}