
using Api.Endpoints;
using Api.Setup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace E2ETests.Setup;

public sealed class TestApplicationHost : IAsyncLifetime
{
    private WebApplication? _app;

    public Uri? BaseAddress { get; private set; }

    public async ValueTask InitializeAsync()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = Environments.Development
        });

        builder.Services.AddServices(builder.Environment);

        builder.WebHost.UseKestrel(opt =>
        {
            opt.Listen(System.Net.IPAddress.Loopback, 0);
        });

        _app = builder.Build();

        _app.MapEndpoints();

        _app.AddMiddleware(_app.Environment, _app.Configuration);

        await _app.StartAsync();

        var address = _app?.Services
            .GetRequiredService<IServer>()
            .Features
            .Get<IServerAddressesFeature>()
            ?.Addresses
            ?.First();

        ArgumentNullException.ThrowIfNull(address);

        BaseAddress = new Uri(address);
    }

    public async ValueTask DisposeAsync()
    {
        if (_app != null)
        {
            await _app.StopAsync();
            await _app.DisposeAsync();
        }
    }
}
