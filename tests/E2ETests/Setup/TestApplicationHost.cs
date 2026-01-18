
using Api.Endpoints;
using Api.Setup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace E2ETests.Setup;

public sealed class ApiTestHost : IAsyncLifetime
{
    private WebApplication? _app = null!;

    public Uri BaseAddress { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        Environment.SetEnvironmentVariable("HTTP_PORTS", null);
        Environment.SetEnvironmentVariable("HTTPS_PORTS", null);

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            Args = [],
            EnvironmentName = "Development",
            ApplicationName = typeof(Program).Assembly.FullName
        });

        builder.Services.AddServices(builder.Environment);

        builder.WebHost
            .UseKestrel()
            .UseUrls("http://127.0.0.1:0");

        _app = builder.Build();

        _app.MapEndpoints();

        _app.AddMiddleware(_app.Environment, _app.Configuration);

        await _app.StartAsync();

        var addresses = _app?.Services
            .GetRequiredService<IServer>()
            .Features
            .Get<IServerAddressesFeature>()
            ?.Addresses;

        var address = addresses?.First();

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
