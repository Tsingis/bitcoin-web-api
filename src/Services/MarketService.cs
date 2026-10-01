using Common.Extensions;
using Microsoft.Extensions.Logging;
using Services.Models;

namespace Services;

public class MarketService(ILogger<MarketService> logger, IMarketClient marketClient) : IMarketService
{
    private readonly ILogger<MarketService> _logger = logger;
    private readonly IMarketClient _marketClient = marketClient;

    public async Task<MarketAnalysis?> GetMarketAnalysis(DateOnly fromDate, DateOnly toDate)
    {
        _logger.LogDebug("Analyzing Bitcoin market from {FromDate} to {ToDate}", fromDate, toDate);
        var data = await _marketClient.GetMarketChartByDateRange(fromDate, toDate).ConfigureAwait(false);

        if (data is null)
        {
            return null;
        }

        var highestTradingVolume = CalculateHighestTradingVolume(data);
        var bestBuyAndSellDates = CalculateBestBuyAndSellDates(data);
        var longestDownwardTrend = CalculateLongestDownwardTrend(data);

        return new MarketAnalysis(
            longestDownwardTrend,
            highestTradingVolume?.Date,
            highestTradingVolume?.Volume,
            bestBuyAndSellDates?.BuyDate,
            bestBuyAndSellDates?.SellDate);
    }

    public async Task<int?> GetLongestDownwardTrend(DateOnly fromDate, DateOnly toDate)
    {
        _logger.LogDebug("Getting longest downward trend for price from {FromDate} to {ToDate}", fromDate, toDate);
        var data = await _marketClient.GetMarketChartByDateRange(fromDate, toDate).ConfigureAwait(false);
        int? longestDownwardPriceTrend = data is null ? null : CalculateLongestDownwardTrend(data);
        _logger.LogDebug("Longest downward price trend {LongestDownwardPriceTrend} days.", longestDownwardPriceTrend);
        return longestDownwardPriceTrend;
    }

    public async Task<(DateOnly Date, decimal Volume)?> GetHighestTradingVolume(DateOnly fromDate, DateOnly toDate)
    {
        _logger.LogDebug("Getting highest trading volume and date from {FromDate} to {ToDate}", fromDate, toDate);
        var data = await _marketClient.GetMarketChartByDateRange(fromDate, toDate).ConfigureAwait(false);

        var trade = data is null ? null : CalculateHighestTradingVolume(data);

        if (trade is not null)
        {
            _logger.LogDebug("Highest trade volume {Volume} on {Date}.", trade.Value.Volume, trade.Value.Date);
        }
        return trade;
    }

    public async Task<(DateOnly SellDate, DateOnly BuyDate)?> GetBestBuyAndSellDates(DateOnly fromDate, DateOnly toDate)
    {
        _logger.LogDebug("Getting best buy and sell dates from {FromDate} to {ToDate}", fromDate, toDate);
        var data = await _marketClient.GetMarketChartByDateRange(fromDate, toDate).ConfigureAwait(false);

        var trade = data is null ? null : CalculateBestBuyAndSellDates(data);

        if (trade is not null)
        {
            _logger.LogDebug("Best buy date {BuyDate} and best sell date {SellDate}.", trade.Value.BuyDate, trade.Value.SellDate);
        }
        return trade;
    }

    private static int CalculateLongestDownwardTrend(List<MarketChartPoint> data)
    {
        var prices = data.Select(x => x.Price).ToList();
        return prices.LongestConsecutiveDecreasingSubset();
    }

    private static (DateOnly Date, decimal Volume)? CalculateHighestTradingVolume(List<MarketChartPoint> data)
    {
        var highestByTotalVolume = data.MaxBy(x => x.TotalVolume);

        return highestByTotalVolume is null
            ? null
            : (highestByTotalVolume.Date.ToDateOnly(), highestByTotalVolume.TotalVolume);
    }

    private static (DateOnly SellDate, DateOnly BuyDate)? CalculateBestBuyAndSellDates(List<MarketChartPoint> data)
    {
        var prices = data.Select(x => x.Price).ToList();
        if (prices.IsOrderedDecreasing())
        {
            return null;
        }

        var lowestByPrice = data.MinBy(x => x.Price);
        var highestByPrice = data.MaxBy(x => x.Price);

        return lowestByPrice is null || highestByPrice is null
            ? null
            : (highestByPrice.Date.ToDateOnly(), lowestByPrice.Date.ToDateOnly());
    }
}
