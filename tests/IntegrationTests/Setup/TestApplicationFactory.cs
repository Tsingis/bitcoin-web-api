using Common;
using MartinCostello.Logging.XUnit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Xunit;

namespace IntegrationTests.Setup;

public class TestApplicationFactory(Fixture fixture) : WebApplicationFactory<Program>, ITestOutputHelperAccessor
{
    public ITestOutputHelper? OutputHelper { get; set; }

    public virtual void ClearOutputHelper()
    {
        OutputHelper = null;
    }

    public virtual void SetOutputHelper(ITestOutputHelper value)
    {
        OutputHelper = value;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        if (EnvVarAccessors.UseMocking)
        {
            builder?.UseSetting(EnvVarKeys.MarketClientUrl, $"http://localhost:{fixture.GetPort()}");
        }

        builder?.ConfigureLogging(x => x.ClearProviders().AddXUnit().SetMinimumLevel(LogLevel.Debug));
    }
}
