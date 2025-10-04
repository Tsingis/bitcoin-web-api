using Api.Endpoints;
using Api.Setup;
using Serilog;

DotNetEnv.Env.TraversePath().Load();

try
{
    Log.Logger = LoggingConfigurationExtensions.CreateBootstrapLogger();

    Log.Information("Starting up the application");

    var builder = WebApplication.CreateBuilder(args);

    builder.Configuration.AddConfiguration(builder.Environment);

    builder.Services.AddServices(builder.Environment);

    builder.Host.UseSerilog((ctx, services, loggerConfig) =>
        loggerConfig.AddLoggingConfiguration(ctx.Configuration, builder.Environment),
        writeToProviders: false,
        preserveStaticLogger: false
    );

    var app = builder.Build();

    app.MapEndpoints();

    app.AddMiddleware(app.Environment, app.Configuration);

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
