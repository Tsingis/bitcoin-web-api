namespace Api.Endpoints;

public record LongestDownwardTrendResponse(int Days);
public record HighestTradingVolumeResponse(DateOnly Date, decimal Volume);
public record BuyAndSellResponse(DateOnly BuyDate, DateOnly SellDate);
