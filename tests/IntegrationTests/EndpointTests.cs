using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using Api.Endpoints;
using IntegrationTests.Setup;
using Shouldly;
using Xunit;

namespace IntegrationTests;

public sealed class EndpointsTests(WiremockFixture fixture, ITestOutputHelper outputHelper) : TestBase(fixture, outputHelper)
{
    public static TheoryData<string?, string?, HttpStatusCode> Cases =>
    new()
    {
        {Constants.s_today.ToString(Constants.DateFormat, CultureInfo.InvariantCulture), Constants.s_today.AddMonths(1).ToString(Constants.DateFormat, CultureInfo.InvariantCulture), HttpStatusCode.NoContent},
        {Constants.s_mockDate.ToString(Constants.DateFormat, CultureInfo.InvariantCulture), Constants.s_mockDate.AddDays(11).ToString(Constants.DateFormat, CultureInfo.InvariantCulture), HttpStatusCode.OK},
        {Constants.s_mockDate.AddYears(-1).AddDays(-1).ToString(Constants.DateFormat, CultureInfo.InvariantCulture), Constants.s_mockDate.ToString(Constants.DateFormat, CultureInfo.InvariantCulture), HttpStatusCode.Unauthorized}, //Unauthorized for over 365 days old queries
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
            var data = await result.Content.ReadFromJsonAsync<LongestDownwardTrendResponse>(cancellationToken: ct);
            data.ShouldNotBeNull();
        }
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public async Task HighestTradingVolume(string? fromDate, string? toDate, HttpStatusCode status)
    {
        var ct = TestContext.Current.CancellationToken;
        var url = new Uri($"{Constants.BaseUrl}/highestradingvolume?fromDate={fromDate}&toDate={toDate}", UriKind.Relative);
        var result = await _client.GetAsync(url, cancellationToken: ct);
        result.StatusCode.ShouldBeOneOf(status, HttpStatusCode.TooManyRequests);

        if (result.StatusCode == HttpStatusCode.OK)
        {
            var data = await result.Content.ReadFromJsonAsync<HighestTradingVolumeResponse>(cancellationToken: ct);
            data.ShouldNotBeNull();
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
            var data = await result.Content.ReadFromJsonAsync<BuyAndSellResponse>(cancellationToken: ct);
            data.ShouldNotBeNull();
        }
    }

    [Fact]
    public async Task Health()
    {
        var ct = TestContext.Current.CancellationToken;
        var result = await _client.GetAsync(new Uri("/health", UriKind.Relative), cancellationToken: ct);
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
