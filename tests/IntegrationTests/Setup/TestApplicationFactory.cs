using Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.XUnit3;
using Xunit;

namespace IntegrationTests.Setup;



public sealed class TestApplicationFactory(Fixture fixture) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        if (EnvVarAccessors.UseMockServer)
        {
            builder?.ConfigureAppConfiguration((context, builder) =>
            {
                var settings = new Dictionary<string, string?>
                {
                    { EnvVarKeys.MarketClientUrl, $"http://localhost:{fixture.GetPort()}" }
                };
                builder.AddInMemoryCollection(settings);
            });
        }
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddSingleton(Options.Create(new XUnit3TestOutputSinkOptions()));
            services.AddSingleton<XUnit3TestOutputSink>();
        });

        builder.UseSerilog((ctx, sp, config) =>
            config.MinimumLevel.Is(GetLogEventLevel())
                .MinimumLevel.Override("System", LogEventLevel.Error)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Error)
                .WriteTo.XUnit3TestOutput(sp.GetRequiredService<XUnit3TestOutputSink>())
            );

        return base.CreateHost(builder);
    }

    public void SetTestOutputHelper(ITestOutputHelper testOutputHelper) =>
        Services.GetRequiredService<XUnit3TestOutputSink>().TestOutputHelper = testOutputHelper;

    private static LogEventLevel GetLogEventLevel()
    {
        var logLevel = Environment.GetEnvironmentVariable(EnvVarKeys.TestingLogLevel);
        return logLevel?.ToUpperInvariant() switch
        {
            "VERBOSE" => LogEventLevel.Verbose,
            "DEBUG" => LogEventLevel.Debug,
            "INFORMATION" => LogEventLevel.Information,
            "WARNING" => LogEventLevel.Warning,
            "ERROR" => LogEventLevel.Error,
            "FATAL" => LogEventLevel.Fatal,
            _ => LogEventLevel.Information
        };
    }
}
