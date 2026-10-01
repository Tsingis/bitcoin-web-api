namespace Services.Models;

public sealed record MarketAnalysis(
    int LongestDownwardTrendDays,
    DateOnly? HighestTradingVolumeDate,
    decimal? HighestTradingVolume,
    DateOnly? BestBuyDate,
    DateOnly? BestSellDate);
