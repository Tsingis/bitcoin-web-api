using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace IntegrationTests;

public sealed class ApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public ValueTask InitializeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public new async ValueTask DisposeAsync()
    {
        await base.DisposeAsync().ConfigureAwait(false);
    }
}
