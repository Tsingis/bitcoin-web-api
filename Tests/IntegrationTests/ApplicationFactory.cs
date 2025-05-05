using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using MartinCostello.Logging.XUnit;

namespace IntegrationTests;

public sealed class ApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime, ITestOutputHelperAccessor
{

    public ITestOutputHelper? OutputHelper { get; set; }

    public void ClearOutputHelper()
        => OutputHelper = null;

    public void SetOutputHelper(ITestOutputHelper? value)
        => OutputHelper = value;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(loggingBuilder =>
            loggingBuilder.ClearProviders().AddXUnit(this)
        );
    }

    public ValueTask InitializeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public new async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
    }
}
