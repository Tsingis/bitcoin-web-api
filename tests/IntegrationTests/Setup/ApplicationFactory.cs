using Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace IntegrationTests.Setup;

public sealed class ApplicationFactory(Fixture fixture) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        if (bool.Parse(Environment.GetEnvironmentVariable(EnvironmentVariable.UseMocking) ?? "false"))
        {
            Environment.SetEnvironmentVariable(EnvironmentVariable.MarketClientUrl, $"http://localhost:{fixture.GetPort()}");
        }
    }
}
