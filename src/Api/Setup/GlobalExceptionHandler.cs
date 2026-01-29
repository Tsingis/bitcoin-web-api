using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Api.Setup;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, ProblemDetailsFactory problemDetailsFactory, IWebHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        var statusCode = (int)HttpStatusCode.InternalServerError;
        var title = "Unexpected Error";
        var detail = "Unhandled exception while processing request";

        switch (exception)
        {
            case BadHttpRequestException badRequestEx:
                statusCode = badRequestEx.StatusCode;
                title = "Bad Request";
                detail = "Invalid request parameter format.";
                break;

            case HttpRequestException httpEx when httpEx.StatusCode == HttpStatusCode.TooManyRequests:
                statusCode = (int)httpEx.StatusCode;
                title = "Too Many Requests";
                detail = "Your hit request rate limit. Try again soon";
                break;

            case HttpRequestException httpEx when httpEx.StatusCode == HttpStatusCode.Unauthorized:
                statusCode = (int)httpEx.StatusCode;
                title = "Unauthorized";
                detail = "Query spanning over 365 days is not allowed.";
                break;
        }

        if (statusCode >= 500)
        {
            logger.LogError(exception, "Unhandled exception while processing request");
        }

        var problemDetails = problemDetailsFactory.CreateProblemDetails(httpContext, statusCode, title, detail);

        if (environment.IsDevelopment())
        {
            problemDetails.Extensions["exception"] = new
            {
                type = exception?.GetType().FullName,
                message = exception?.Message,
                stackTrace = exception?.StackTrace,
                innerException = exception?.InnerException?.Message
            };
        }

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken)
            .ConfigureAwait(false);

        return true;
    }
}
