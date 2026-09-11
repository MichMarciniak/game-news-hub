using GameNewsHub.Tests.Integration.Fixtures;

namespace GameNewsHub.Tests.Integration.Bases;

[Collection("integration")]
public abstract class BaseIntegrationTest
{
    protected readonly HttpClient Client;
    protected readonly IntegrationTestFixture Fixture;

    protected BaseIntegrationTest(IntegrationTestFixture fixture)
    {
        Fixture = fixture;
        Client = fixture.Factory.CreateClient();
    }
}