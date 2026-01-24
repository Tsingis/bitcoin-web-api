using System.Globalization;
using Common;
using Serilog;
using Serilog.Exceptions;
using Serilog.Extensions.Hosting;
using Serilog.Formatting;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Display;

namespace Api.Setup;

internal static class LoggingConfigurationExtensions
{
    const string LogFormat = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}";

    internal static ReloadableLogger CreateBootstrapLogger()
    {
        var logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console(outputTemplate: LogFormat, formatProvider: CultureInfo.InvariantCulture)
            .CreateBootstrapLogger();

        return logger;
    }

    internal static void AddLoggingConfiguration(this LoggerConfiguration loggerConfig, IConfiguration config, IWebHostEnvironment environment)
    {
        ITextFormatter formatter = new MessageTemplateTextFormatter(outputTemplate: LogFormat, formatProvider: CultureInfo.InvariantCulture);

        if (EnvVarAccessors.UseJsonFormatting)
        {
            formatter = new CompactJsonFormatter();
        }

        loggerConfig?
            .ReadFrom.Configuration(config)
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .WriteTo.Async(x => x.Console(formatter));

        if (environment.IsProduction())
        {
            var connString = config?["ApplicationInsights:ConnectionString"];
            loggerConfig?.WriteTo.ApplicationInsights(connString, TelemetryConverter.Traces);
        }
    }
}
