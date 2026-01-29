using System.Text.Json;
using Common;
using Common.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Services.Models;
using Services.Utility;

namespace Services;

public class MarketClient(ILogger<MarketClient> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory) : IMarketClient
{
    private readonly ILogger<MarketClient> _logger = logger;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    private readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };

    public async Task<List<MarketChartPoint>?> GetMarketChartByDateRange(DateOnly fromDate, DateOnly toDate)
    {
        string? url = configuration.GetValue<string>(EnvVarKeys.ApiUrl);
        if (string.IsNullOrEmpty(url))
        {
            throw new InvalidOperationException($"Environment variable {EnvVarKeys.ApiUrl} is not set");
        }

        Dictionary<string, string?> parameters = QueryHelper.CreateQueryParams(fromDate, toDate, "eur");
        string queryUrl = QueryHelpers.AddQueryString(url, parameters);
        using var request = new HttpRequestMessage(HttpMethod.Get, queryUrl);
        request.Headers.Add("x-cg-demo-api-key", configuration.GetValue("api-key", string.Empty));

        using (var httpClient = _httpClientFactory.CreateClient())
        {
            HttpResponseMessage response = await httpClient.SendAsync(request).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var marketChart = JsonSerializer.Deserialize<MarketChart>(json, _options);

            var points = MarketChartHelper.MapMarketChartToMarketChartPoints(marketChart);

            if (points is null)
            {
                _logger.LogWarning("Market chart data not found");
                return null;
            }

            var data = MarketChartHelper.GetEarliestMarketChartPointsByDate(points);
            return data;
        }
    }
}
