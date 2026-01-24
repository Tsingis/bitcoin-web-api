using Xunit;

namespace IntegrationTests.Setup;

public abstract class TestBase : IAsyncLifetime
{
    internal readonly TestFactory _factory;
    internal readonly HttpClient _client;

    protected TestBase(WiremockFixture wiremock, ITestOutputHelper outputHelper,
        bool useOutputCache = true, bool useRateLimiter = true)
    {
        _factory = new TestFactory(wiremock, useOutputCache, useRateLimiter);
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
