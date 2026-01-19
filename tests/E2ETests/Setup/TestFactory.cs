using E2ETests.Setup;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Xunit;

[assembly: AssemblyFixture(typeof(TestFactory))]

namespace E2ETests.Setup;

public sealed class TestFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public Uri? BaseAddress { get; private set; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(Environments.Development);
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseSerilog((ctx, sp, config) =>
            config.MinimumLevel.Error().WriteTo.Console(),
            preserveStaticLogger: true,
            writeToProviders: false
            );

        return base.CreateHost(builder);
    }

    public async ValueTask InitializeAsync()
    {
        UseKestrel(options =>
        {
            options.Listen(System.Net.IPAddress.Loopback, 0);
        });

        var address = Services
            .GetRequiredService<IServer>()
            .Features
            .Get<IServerAddressesFeature>()
            ?.Addresses
            ?.First();

        ArgumentNullException.ThrowIfNull(address);

        BaseAddress = new Uri(address);
    }
}
