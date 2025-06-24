using Xunit;

namespace IntegrationTests;

public abstract class IntegrationTestBase : IClassFixture<ApplicationFactory>, IAsyncLifetime
{
    internal readonly ApplicationFactory _factory;
    internal readonly HttpClient _client;

    protected IntegrationTestBase(ApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    public virtual ValueTask InitializeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask DisposeAsync()
    {
        _client.Dispose();
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
}
