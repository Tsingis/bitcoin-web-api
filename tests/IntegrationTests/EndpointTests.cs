using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using Api.Endpoints.Bitcoin;
using Common;
using IntegrationTests.Setup;
using Shouldly;
using Xunit;

namespace IntegrationTests;

public sealed class EndpointsTests(WiremockFixture fixture, ITestOutputHelper outputHelper) : TestBase(fixture, outputHelper)
{
    public static TheoryData<string?, string?, HttpStatusCode> Cases =>
    new()
    {
        {Constants.Today.ToString(Constants.DateFormat, CultureInfo.InvariantCulture), Constants.Today.AddMonths(1).ToString(Constants.DateFormat, CultureInfo.InvariantCulture), HttpStatusCode.NoContent},
        {Constants.StartMockDate.ToString(Constants.DateFormat, CultureInfo.InvariantCulture), Constants.EndMockDate.ToString(Constants.DateFormat, CultureInfo.InvariantCulture), HttpStatusCode.OK},
        {Constants.StartMockDate.AddYears(-1).AddDays(-1).ToString(Constants.DateFormat, CultureInfo.InvariantCulture), Constants.StartMockDate.ToString(Constants.DateFormat, CultureInfo.InvariantCulture), HttpStatusCode.Unauthorized}, //Unauthorized for over 365 days old queries
        {"", null, HttpStatusCode.BadRequest},
    };

    [Theory]
    [MemberData(nameof(Cases))]
    public async Task LongestDownwardTrend(string? fromDate, string? toDate, HttpStatusCode status)
    {
        var ct = TestContext.Current.CancellationToken;
        var url = new Uri($"{Constants.BaseUrl}/longestdownwardtrend?fromDate={fromDate}&toDate={toDate}", UriKind.Relative);
        var result = await _client.GetAsync(url, cancellationToken: ct);
        result.StatusCode.ShouldBeOneOf(status, HttpStatusCode.TooManyRequests);

        if (result.StatusCode == HttpStatusCode.OK)
        {
            var data = await result.Content.ReadFromJsonAsync<LongestDownwardTrendResponse>(ct);
            data.ShouldNotBeNull();
            data.Days.ShouldBe(3);
        }
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public async Task HighestTradingVolume(string? fromDate, string? toDate, HttpStatusCode status)
    {
        var ct = TestContext.Current.CancellationToken;
        var url = new Uri($"{Constants.BaseUrl}/highesttradingvolume?fromDate={fromDate}&toDate={toDate}", UriKind.Relative);
        var result = await _client.GetAsync(url, cancellationToken: ct);
        result.StatusCode.ShouldBeOneOf(status, HttpStatusCode.TooManyRequests);

        if (result.StatusCode == HttpStatusCode.OK)
        {
            var data = await result.Content.ReadFromJsonAsync<HighestTradingVolumeResponse>(ct);
            data.ShouldNotBeNull();
            data.Date.ShouldBe(new DateOnly(2025, 9, 6));
            data.Volume.ShouldBe(48013311912.10m, 0.01m);
        }
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public async Task BuyAndSell(string? fromDate, string? toDate, HttpStatusCode status)
    {
        var ct = TestContext.Current.CancellationToken;
        var url = new Uri($"{Constants.BaseUrl}/buyandsell?fromDate={fromDate}&toDate={toDate}", UriKind.Relative);
        var result = await _client.GetAsync(url, cancellationToken: ct);
        result.StatusCode.ShouldBeOneOf(status, HttpStatusCode.TooManyRequests);

        if (result.StatusCode == HttpStatusCode.OK)
        {
            var data = await result.Content.ReadFromJsonAsync<BuyAndSellResponse>(ct);
            data.ShouldNotBeNull();
            data.BuyDate.ShouldBe(new DateOnly(2025, 9, 1));
            data.SellDate.ShouldBe(new DateOnly(2025, 9, 4));
        }
    }

    [Fact]
    public async Task Health()
    {
        var ct = TestContext.Current.CancellationToken;
        var result = await _client.GetAsync(new Uri("/health", UriKind.Relative), ct);
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
