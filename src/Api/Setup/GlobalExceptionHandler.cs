using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Api.Setup;

public sealed class GlobalExceptionHandler(ProblemDetailsFactory problemDetailsFactory, IWebHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        var statusCode = (int)HttpStatusCode.InternalServerError;
        var title = "Unexpected Error";
        var detail = "An unhandled exception occurred.";

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
                detail = "The external service rate-limited your request.";
                break;

            case HttpRequestException httpEx when httpEx.StatusCode == HttpStatusCode.Unauthorized:
                statusCode = (int)httpEx.StatusCode;
                title = "Unauthorized";
                detail = "Query spanning over 365 days is not allowed.";
                break;
        }

        var problemDetails = problemDetailsFactory.CreateProblemDetails(
            httpContext,
            statusCode,
            title: title,
            detail: detail);

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
