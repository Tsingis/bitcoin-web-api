using System.ComponentModel;
using ModelContextProtocol.Server;
using Services;
using Services.Models;

namespace Api.Mcp;

[McpServerToolType]
public sealed class BitcoinMarketTools
{
    [McpServerTool(UseStructuredContent = true), Description("Analyzes Bitcoin market activity over a date range and returns its price trend, highest-volume day, and best buy/sell dates in one result.")]
    public static Task<MarketAnalysis?> AnalyzeBitcoinMarket(
        [Description("First date to include, in ISO 8601 format.")] DateOnly fromDate,
        [Description("Last date to include, in ISO 8601 format.")] DateOnly toDate,
        IMarketService marketService)
    {
        ArgumentNullException.ThrowIfNull(marketService);
        return marketService.GetMarketAnalysis(fromDate, toDate);
    }
}
