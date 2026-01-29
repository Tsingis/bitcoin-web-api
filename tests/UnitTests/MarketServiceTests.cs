using Common.Extensions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Services;
using Services.Models;
using Shouldly;
using Xunit;

namespace UnitTests;

public class MarketServiceTests
{
    public DateOnly FromDate { get; }
    public DateOnly ToDate { get; }
    public DateOnly ToDateExtension { get; }
    public DateOnly ToDateNullExtension { get; }
    private readonly MarketService _marketService;

    public MarketServiceTests()
    {
        var date = new DateTimeOffset(2021, 1, 1, 12, 0, 0, TimeSpan.Zero);

        FromDate = date.ToDateOnly();
        ToDate = date.AddDays(4).ToDateOnly();
        ToDateExtension = date.AddDays(7).ToDateOnly();
        ToDateNullExtension = date.AddDays(10).ToDateOnly();

        var marketClient = Substitute.For<IMarketClient>();

        marketClient.GetMarketChartByDateRange(FromDate, ToDate)
            .Returns(CreateMarketChartPoints(date));

        marketClient.GetMarketChartByDateRange(ToDate, ToDateExtension)
            .Returns(CreateExtensionPoints(date));

        marketClient.GetMarketChartByDateRange(ToDateExtension, ToDateNullExtension)
            .Returns((List<MarketChartPoint>?)null);

        _marketService = new MarketService(
            new NullLogger<MarketService>(),
            marketClient
        );
    }

    private static List<MarketChartPoint> CreateMarketChartPoints(DateTimeOffset start)
    {
        return [
            new() { Date = start, Price = 100, MarketCap = 100, TotalVolume = 100 },
            new() { Date = start.AddDays(1), Price = 90, MarketCap = 100, TotalVolume = 200 },
            new() { Date = start.AddDays(2), Price = 80, MarketCap = 100, TotalVolume = 300 },
            new() { Date = start.AddDays(3), Price = 70, MarketCap = 100, TotalVolume = 400 },
            new() { Date = start.AddDays(4), Price = 500, MarketCap = 100, TotalVolume = 500 },
        ];
    }

    private static List<MarketChartPoint> CreateExtensionPoints(DateTimeOffset start)
    {
        return [
            new() { Date = start.AddDays(5), Price = 50, MarketCap = 100, TotalVolume = 50 },
            new() { Date = start.AddDays(6), Price = 40, MarketCap = 100, TotalVolume = 40 },
            new() { Date = start.AddDays(7), Price = 30, MarketCap = 100, TotalVolume = 30 },
        ];
    }

    [Fact]
    public async Task LongestDownwardTrend_ReturnsValue()
    {
        var result = await _marketService.GetLongestDownwardTrend(FromDate, ToDate);

        result.ShouldNotBeNull();
        result.ShouldBe(3);
    }

    [Fact]
    public async Task LongestDownwardTrend_NoData_ReturnsNull()
    {
        var result = await _marketService.GetLongestDownwardTrend(ToDateExtension, ToDateNullExtension);

        result.ShouldBeNull();
    }

    [Fact]
    public async Task HighestTradingVolume_ReturnsValue()
    {
        var result = await _marketService.GetHighestTradingVolume(FromDate, ToDate);

        result.ShouldNotBeNull();
        result?.Date.ShouldBe(new DateOnly(2021, 1, 5));
        result?.Volume.ShouldBe(500m);
    }

    [Fact]
    public async Task HighestTradingVolume_NoData_ReturnsNull()
    {
        var result = await _marketService.GetHighestTradingVolume(ToDateExtension, ToDateNullExtension);

        result.ShouldBeNull();
    }

    [Fact]
    public async Task BestBuyAndSellDates_ReturnsValue()
    {
        var result = await _marketService.GetBestBuyAndSellDates(FromDate, ToDate);

        result.ShouldNotBeNull();
        result?.BuyDate.ShouldBe(new DateOnly(2021, 1, 4));
        result?.SellDate.ShouldBe(new DateOnly(2021, 1, 5));
    }

    [Fact]
    public async Task BestBuyAndSellDates_OnlyDecreasing_ReturnsNull()
    {
        var result = await _marketService.GetBestBuyAndSellDates(ToDate, ToDateExtension);

        result.ShouldBeNull();
    }

    [Fact]
    public async Task BestBuyAndSellDates_NoData_ReturnsNull()
    {
        var result = await _marketService.GetBestBuyAndSellDates(ToDateExtension, ToDateNullExtension);

        result.ShouldBeNull();
    }
}
