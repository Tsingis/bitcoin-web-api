using System.Globalization;
using Api.Setup;
using Azure.Identity;
using Serilog;

DotNetEnv.Env.TraversePath().Load();

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
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

    if (builder.Environment.IsProduction())
    {
        var keyVaultName = Environment.GetEnvironmentVariable("KEY_VAULT_NAME");
        var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");
        builder.Configuration.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config)
            .WriteTo.Console(outputTemplate: logFormat, formatProvider: CultureInfo.InvariantCulture)
            .WriteTo.ApplicationInsights(config["ApplicationInsightsConnectionString"], TelemetryConverter.Traces)
            .CreateLogger();
    }

    builder.Services.ConfigureServices();

    var app = builder.Build();

    app.ConfigureEndpoints();

    app.ConfigureMiddleware(app.Environment);

    await app.RunAsync();
}
catch (Exception ex)
{
    const string message = "Application terminated unexpectedly";
    Log.Fatal(ex, message);
    throw new InvalidOperationException(message, ex);
}
finally
{
    await Log.CloseAndFlushAsync();
}

public partial class Program { } // Reference for tests
