using System.Net;
using Asp.Versioning;
using Microsoft.AspNetCore.Http.HttpResults;
using Services;

namespace Api.Setup;

internal static class ApiEndpoints
{
    public static void ConfigureEndpoints(this WebApplication app)
    {
        var apiVersionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();

        var group = app
            .MapGroup("api/v{version:apiVersion}")
            .WithApiVersionSet(apiVersionSet);

        group.MapGet("/longestdownwardtrend",
            async Task<Results<Ok<LongestDownwardTrendResponse>, NoContent,
                BadRequest, StatusCodeHttpResult, UnauthorizedHttpResult, ProblemHttpResult>>
            (IMarketService service, DateOnly fromDate, DateOnly toDate) =>
            {
                try
                {
                    var result = await service.GetLongestDownwardTrend(fromDate, toDate).ConfigureAwait(false);

                    if (result is null)
                    {
                        return TypedResults.NoContent();
                    }

                    return TypedResults.Ok(new LongestDownwardTrendResponse(result.Value));
                }
                catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    return TypedResults.StatusCode((int)HttpStatusCode.TooManyRequests);
                }
                catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return TypedResults.Problem(detail: "Query spanning over 365 days", statusCode: (int)HttpStatusCode.Unauthorized);
                }
                catch (HttpRequestException)
                {
                    return TypedResults.Problem(statusCode: (int)HttpStatusCode.InternalServerError);
                }
            })
            .WithDescription("Get longest downward trend in days between given dates")
            .ProducesProblem((int)HttpStatusCode.TooManyRequests)
            .ProducesProblem((int)HttpStatusCode.Unauthorized)
            .ProducesProblem((int)HttpStatusCode.InternalServerError);

        group.MapGet("/highestradingvolume",
            async Task<Results<Ok<HighestTradingVolumeResponse>, NoContent,
                BadRequest, StatusCodeHttpResult, UnauthorizedHttpResult, ProblemHttpResult>>
            (IMarketService service, DateOnly fromDate, DateOnly toDate) =>
            {
                try
                {
                    var result = await service.GetHighestTradingVolume(fromDate, toDate).ConfigureAwait(false);
                    if (result is null)
                    {
                        return TypedResults.NoContent();
                    }
                    return TypedResults.Ok(new HighestTradingVolumeResponse
                    (
                        result.Value.Date,
                        result.Value.Volume
                    ));
                }
                catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    return TypedResults.StatusCode((int)HttpStatusCode.TooManyRequests);
                }
                catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return TypedResults.Problem(detail: "Query spanning over 365 days", statusCode: (int)HttpStatusCode.Unauthorized);
                }
                catch (HttpRequestException)
                {
                    return TypedResults.Problem(statusCode: (int)HttpStatusCode.InternalServerError);
                }
            })
            .WithDescription("Get the date with the highest trading volume between given dates")
            .ProducesProblem((int)HttpStatusCode.TooManyRequests)
            .ProducesProblem((int)HttpStatusCode.Unauthorized)
            .ProducesProblem((int)HttpStatusCode.InternalServerError);

        group.MapGet("/buyandsell",
            async Task<Results<Ok<BuyAndSellResponse>, NoContent,
                BadRequest, StatusCodeHttpResult, UnauthorizedHttpResult, ProblemHttpResult>>
            (IMarketService service, DateOnly fromDate, DateOnly toDate) =>
            {
                try
                {
                    var result = await service.GetBestBuyAndSellDates(fromDate, toDate).ConfigureAwait(false);
                    if (result is null)
                    {
                        return TypedResults.NoContent();
                    }
                    return TypedResults.Ok(new BuyAndSellResponse
                    (
                        result.Value.BuyDate,
                        result.Value.SellDate
                    ));
                }
                catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    return TypedResults.StatusCode((int)HttpStatusCode.TooManyRequests);
                }
                catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return TypedResults.Problem(detail: "Query spanning over 365 days", statusCode: (int)HttpStatusCode.Unauthorized);
                }
                catch (HttpRequestException)
                {
                    return TypedResults.Problem(statusCode: (int)HttpStatusCode.InternalServerError);
                }
            })
            .WithDescription("Get pair of dates when it is best to buy and sell between given dates")
            .ProducesProblem((int)HttpStatusCode.TooManyRequests)
            .ProducesProblem((int)HttpStatusCode.Unauthorized)
            .ProducesProblem((int)HttpStatusCode.InternalServerError);
    }
}

public record LongestDownwardTrendResponse(int Days);
public record HighestTradingVolumeResponse(DateOnly Date, decimal Volume);
public record BuyAndSellResponse(DateOnly BuyDate, DateOnly SellDate);
