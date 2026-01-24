using System.Net;
using Api.Setup;
using Asp.Versioning;
using Microsoft.AspNetCore.Http.HttpResults;
using Services;

namespace Api.Endpoints;

internal static class Endpoints
{
    public static void MapEndpoints(this WebApplication app)
    {
        var apiVersionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();

        var group = app
            .MapGroup("api/v{version:apiVersion}")
            .WithApiVersionSet(apiVersionSet);

        group.MapGet("/longestdownwardtrend",
            async Task<Results<Ok<LongestDownwardTrendResponse>, NoContent, BadRequest>>
            (IMarketService service, DateOnly fromDate, DateOnly toDate) =>
            {
                var result = await service.GetLongestDownwardTrend(fromDate, toDate).ConfigureAwait(false);

                if (result is null)
                {
                    return TypedResults.NoContent();
                }

                return TypedResults.Ok(new LongestDownwardTrendResponse(result.Value));
            })
            .WithDescription("Get longest downward trend in days between given dates")
            .WithMetadata(new UseDateExamplesAttribute())
            .ProducesProblem((int)HttpStatusCode.TooManyRequests)
            .ProducesProblem((int)HttpStatusCode.Unauthorized)
            .ProducesProblem((int)HttpStatusCode.InternalServerError);

        group.MapGet("/highestradingvolume",
            async Task<Results<Ok<HighestTradingVolumeResponse>, NoContent, BadRequest>>
            (IMarketService service, DateOnly fromDate, DateOnly toDate) =>
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
            })
            .WithDescription("Get the date with the highest trading volume between given dates")
            .WithMetadata(new UseDateExamplesAttribute())
            .ProducesProblem((int)HttpStatusCode.TooManyRequests)
            .ProducesProblem((int)HttpStatusCode.Unauthorized)
            .ProducesProblem((int)HttpStatusCode.InternalServerError);

        group.MapGet("/buyandsell",
            async Task<Results<Ok<BuyAndSellResponse>, NoContent, BadRequest>>
            (IMarketService service, DateOnly fromDate, DateOnly toDate) =>
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
            })
            .WithDescription("Get pair of dates when it is best to buy and sell between given dates")
            .WithMetadata(new UseDateExamplesAttribute())
            .ProducesProblem((int)HttpStatusCode.TooManyRequests)
            .ProducesProblem((int)HttpStatusCode.Unauthorized)
            .ProducesProblem((int)HttpStatusCode.InternalServerError);
    }
}
