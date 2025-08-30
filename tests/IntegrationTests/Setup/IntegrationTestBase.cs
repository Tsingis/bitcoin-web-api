using Xunit;

namespace IntegrationTests.Setup;

public abstract class IntegrationTestBase : IAsyncLifetime
{
    internal readonly ApplicationFactory _factory;
    internal readonly HttpClient _client;

    protected IntegrationTestBase(Fixture fixture)
    {
        _factory = new ApplicationFactory(fixture);
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
