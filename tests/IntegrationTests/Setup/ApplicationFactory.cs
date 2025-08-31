using Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace IntegrationTests.Setup;

public sealed class ApplicationFactory(Fixture fixture) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        if (EnvVarAccessors.UseMocking)
        {
            builder?.UseSetting(EnvVarKeys.MarketClientUrl, $"http://localhost:{fixture.GetPort()}");
        }
    }
}
