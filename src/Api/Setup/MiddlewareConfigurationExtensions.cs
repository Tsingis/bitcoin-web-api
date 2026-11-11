using Common;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;

namespace Api.Setup;

internal static class MiddlewareConfigurationExtensions
{
    internal static void AddMiddleware(this WebApplication app, IWebHostEnvironment environment, IConfiguration configuration)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.GetLevel = (context, _, exception) =>
            {
                if (exception is not null || context.Response.StatusCode >= StatusCodes.Status500InternalServerError)
                {
                    return LogEventLevel.Error;
                }

                if (context.Response.StatusCode >= StatusCodes.Status400BadRequest)
                {
                    return LogEventLevel.Debug;
                }

                return LogEventLevel.Information;
            };
        });

        app.UseExceptionHandler();

        app.UseRouting();

        if (configuration.GetValue(EnvVarKeys.UseOutputCache, true))
        {
            app.UseWhen(
                context =>
                {
                    var path = context.Request.Path.Value ?? string.Empty;
                    return !path.StartsWith("/openapi", StringComparison.OrdinalIgnoreCase)
                        && !path.StartsWith("/scalar", StringComparison.OrdinalIgnoreCase)
                        && !path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase);
                },
                branch => branch.UseOutputCache());
        }

        const string documentName = "/openapi/v1.json";
        const string apiName = "Bitcoin Web API";

        app.MapOpenApi(documentName).WithDocumentPerVersion();

        app.UseSwaggerUI(opt =>
        {
            opt.RoutePrefix = "swagger";
            opt.DocumentTitle = apiName;
            opt.SwaggerEndpoint(documentName, apiName);
        });

        app.MapScalarApiReference(endpointPrefix: "/scalar", opt =>
        {
            opt.Title = apiName;
            opt.ShowSidebar = true;
            opt.DarkMode = true;
        });

        app.MapHealthChecks("health");

        if (environment.IsProduction())
        {
            app.UseHsts();
            app.UseHttpsRedirection();
        }

        app.Use(async (context, next) =>
        {
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Append("X-Frame-Options", "DENY");
            context.Response.Headers.Append("Referrer-Policy", "no-referrer");
            await next().ConfigureAwait(false);
        });

        if (configuration.GetValue(EnvVarKeys.UseRateLimiter, true))
        {
            app.UseRateLimiter();
        }
    }
}
