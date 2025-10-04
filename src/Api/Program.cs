using System.Globalization;
using Api.Endpoints;
using Api.Setup;
using Azure.Identity;
using Common;
using Microsoft.ApplicationInsights.Extensibility;
using Serilog;
using Serilog.Exceptions;

DotNetEnv.Env.TraversePath().Load();

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

const string logFormat = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}";

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(config)
    .WriteTo.Console(outputTemplate: logFormat, formatProvider: CultureInfo.InvariantCulture)
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting up the application");

    var builder = WebApplication.CreateBuilder(args);

    var keyVaultName = builder.Configuration.GetValue<string>(EnvVarKeys.KeyVaultName);
    if (builder.Environment.IsProduction() && !string.IsNullOrEmpty(keyVaultName))
    {
        var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");
        builder.Configuration.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());
    }

    builder.Services.ConfigureServices(builder.Environment);

    builder.Host.UseSerilog((ctx, services, loggerConfig) =>
    {
        loggerConfig
            .ReadFrom.Configuration(ctx.Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .WriteTo.Console(outputTemplate: logFormat, formatProvider: CultureInfo.InvariantCulture);

        if (ctx.HostingEnvironment.IsProduction())
        {
            var tc = new TelemetryConfiguration
            {
                ConnectionString = ctx.Configuration["ApplicationInsights:ConnectionString"]
            };
            loggerConfig.WriteTo.ApplicationInsights(tc, TelemetryConverter.Traces);
        }
    },
        writeToProviders: false,
        preserveStaticLogger: false
    );


    var app = builder.Build();

    app.MapEndpoints();

    app.ConfigureMiddleware(app.Environment, app.Configuration);

    await app.RunAsync().ConfigureAwait(false);
}
catch (Exception ex)
{
    const string message = "Application terminated unexpectedly";
    Log.Fatal(ex, message);
    throw new InvalidOperationException(message, ex);
}
finally
{
    await Log.CloseAndFlushAsync().ConfigureAwait(false);
}

public partial class Program { } // Reference for tests
