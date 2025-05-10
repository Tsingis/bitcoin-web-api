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
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

var keyVaultName = Environment.GetEnvironmentVariable("KEY_VAULT_NAME");

if (!string.IsNullOrEmpty(keyVaultName))
{
    var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");
    builder.Configuration.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());
}

builder.Services.ConfigureServices(builder.Environment, builder.Configuration);

var app = builder.Build();

app.ConfigureEndpoints();

app.ConfigureMiddleware(app.Environment);

await app.RunAsync();

public partial class Program { } // Reference for tests
