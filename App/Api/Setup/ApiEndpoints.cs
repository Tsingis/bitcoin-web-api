using System.Net;
using Asp.Versioning;
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
            async (IMarketService service, DateOnly fromDate, DateOnly toDate) =>
            {
                try
                {
                    var result = await service.GetLongestDownwardTrend(fromDate, toDate);
                    if (result is null)
                    {
                        return Results.NoContent();
                    }
                    return Results.Ok(new LongestDownwardTrendResponse(result.Value));
                }
                catch (HttpRequestException ex)
                {
                    return Results.Problem(statusCode: (int?)ex.StatusCode);
                }
            })
            .WithDescription("Get longest downward trend in days between given dates")
            .Produces<HighestTradingVolumeResponse>((int)HttpStatusCode.OK)
            .Produces((int)HttpStatusCode.NotFound)
            .Produces((int)HttpStatusCode.BadRequest)
            .ProducesProblem((int)HttpStatusCode.TooManyRequests)
            .ProducesProblem((int)HttpStatusCode.InternalServerError);

        group.MapGet("/highestradingvolume",
            async (IMarketService service, DateOnly fromDate, DateOnly toDate) =>
            {
                try
                {
                    var result = await service.GetHighestTradingVolume(fromDate, toDate);
                    if (result is null)
                    {
                        return Results.NoContent();
                    }
                    return Results.Ok(new HighestTradingVolumeResponse
                    (
                        result.Value.Date,
                        result.Value.Volume
                    ));
                }
                catch (HttpRequestException ex)
                {
                    return Results.Problem(statusCode: (int?)ex.StatusCode);
                }
            })
            .WithDescription("Get the date with the highest trading volume between given dates")
            .Produces<HighestTradingVolumeResponse>((int)HttpStatusCode.OK)
            .Produces((int)HttpStatusCode.NotFound)
            .Produces((int)HttpStatusCode.BadRequest)
            .ProducesProblem((int)HttpStatusCode.TooManyRequests)
            .ProducesProblem((int)HttpStatusCode.InternalServerError);

        group.MapGet("/buyandsell",
            async (IMarketService service, DateOnly fromDate, DateOnly toDate) =>
            {
                try
                {
                    var result = await service.GetBestBuyAndSellDates(fromDate, toDate);
                    if (result is null)
                    {
                        return Results.NoContent();
                    }
                    return Results.Ok(new BuyAndSellResponse
                    (
                        result.Value.BuyDate,
                        result.Value.SellDate
                    ));
                }
                catch (HttpRequestException ex)
                {
                    return Results.Problem(statusCode: (int?)ex.StatusCode);
                }
            })
            .WithDescription("Get pair of dates when it is best to buy and sell between given dates")
            .Produces<BuyAndSellResponse>((int)HttpStatusCode.OK)
            .Produces((int)HttpStatusCode.NotFound)
            .Produces((int)HttpStatusCode.BadRequest)
            .ProducesProblem((int)HttpStatusCode.TooManyRequests)
            .ProducesProblem((int)HttpStatusCode.InternalServerError);
    }
}

public record LongestDownwardTrendResponse(int Days);
public record HighestTradingVolumeResponse(DateOnly Date, decimal Volume);
public record BuyAndSellResponse(DateOnly BuyDate, DateOnly SellDate);
