using Scalar.AspNetCore;

namespace Api.Setup;

internal static class ApiMiddleware
{
    public static void ConfigureMiddleware(this WebApplication app, IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseOutputCache();

        app.MapOpenApi();

        app.MapScalarApiReference(opt =>
        {
            opt.Title = "Bitcoin Web API";
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
            await next();
        });

        app.UseRateLimiter();
    }
}
