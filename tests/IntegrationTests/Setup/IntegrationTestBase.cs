using Xunit;

namespace IntegrationTests.Setup;

public abstract class IntegrationTestBase : IAsyncLifetime
{
    internal readonly TestApplicationFactory _factory;
    internal readonly HttpClient _client;

    protected IntegrationTestBase(Fixture fixture, ITestOutputHelper outputHelper)
    {
        _factory = new TestApplicationFactory(fixture);
        _factory.SetTestOutputHelper(outputHelper);
        _client = _factory.CreateClient();
    }

    public virtual ValueTask InitializeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask DisposeAsync()
    {
        _client.Dispose();
        _factory.Dispose();
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
}
