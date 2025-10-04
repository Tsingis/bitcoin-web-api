using Common;
using Scalar.AspNetCore;

namespace Api.Setup;

internal static class ApiMiddleware
{
    public static void ConfigureMiddleware(this WebApplication app, IWebHostEnvironment environment, IConfiguration configuration)
    {
        app.UseExceptionHandler();

        app.UseRouting();

        if (configuration.GetValue(EnvVarKeys.UseOutputCache, true))
        {
            app.UseOutputCache();
        }

        const string documentName = "/openapi/v1.json";
        const string apiName = "Bitcoin Web API v1";

        app.MapOpenApi(documentName);

        app.UseSwaggerUI(opt =>
        {
            opt.RoutePrefix = "swagger";
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
        }

        app.UseHttpsRedirection();

        app.Use(async (context, next) =>
        {
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Append("X-Frame-Options", "DENY");
            context.Response.Headers.Append("Referrer-Policy", "no-referrer");
            await next().ConfigureAwait(false);
        });

        app.UseRateLimiter();
    }
}
