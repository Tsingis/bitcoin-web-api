using System.Net;
using Api.Setup;
using Asp.Versioning;
using Microsoft.AspNetCore.Http.HttpResults;
using Services;

namespace Api.Endpoints;

internal static class Endpoints
{
    internal static void MapEndpoints(this WebApplication app)
    {
        var apiVersionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();

        var api = app
            .MapGroup("api/v{version:apiVersion}")
            .WithApiVersionSet(apiVersionSet);

        var bitcoin = api
            .MapGroup("/bitcoin")
            .WithTags("Bitcoin")
            .WithMetadata(new UseDateExamplesAttribute())
            .ProducesProblem((int)HttpStatusCode.TooManyRequests)
            .ProducesProblem((int)HttpStatusCode.Unauthorized)
            .ProducesProblem((int)HttpStatusCode.InternalServerError);

        bitcoin.MapGet("/longestdownwardtrend", GetLongestDownwardTrend)
            .WithDescription("Get longest downward trend in days between given dates");

        bitcoin.MapGet("/highestradingvolume", GetHighestTradingVolume)
            .WithDescription("Get the date with the highest trading volume between given dates");

        bitcoin.MapGet("/buyandsell", GetBuyAndSell)
            .WithDescription("Get pair of dates when it is best to buy and sell between given dates");
    }

    private static async Task<Results<Ok<LongestDownwardTrendResponse>, NoContent, BadRequest>>
        GetLongestDownwardTrend(IMarketService service, [AsParameters] DateRangeRequest request)
    {
        var result = await service
            .GetLongestDownwardTrend(request.FromDate, request.ToDate)
            .ConfigureAwait(false);

        if (result is null)
        {
            return TypedResults.NoContent();
        }

        return TypedResults.Ok(new LongestDownwardTrendResponse(result.Value));
    }

    private static async Task<Results<Ok<HighestTradingVolumeResponse>, NoContent, BadRequest>>
        GetHighestTradingVolume(IMarketService service, [AsParameters] DateRangeRequest request)
    {
        var result = await service
            .GetHighestTradingVolume(request.FromDate, request.ToDate)
            .ConfigureAwait(false);

        if (result is null)
        {
            return TypedResults.NoContent();
        }

        return TypedResults.Ok(new HighestTradingVolumeResponse(
            result.Value.Date,
            result.Value.Volume));
    }

    private static async Task<Results<Ok<BuyAndSellResponse>, NoContent, BadRequest>>
        GetBuyAndSell(IMarketService service, [AsParameters] DateRangeRequest request)
    {
        var result = await service
            .GetBestBuyAndSellDates(request.FromDate, request.ToDate)
            .ConfigureAwait(false);

        if (result is null)
        {
            return TypedResults.NoContent();
        }

        return TypedResults.Ok(new BuyAndSellResponse(
            result.Value.BuyDate,
            result.Value.SellDate));
    }
}
